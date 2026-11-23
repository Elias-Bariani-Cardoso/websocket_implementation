using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Infra.Sockets
{
    public class BackgroundSocketProcessor : BackgroundService
    {
        private static readonly ConcurrentDictionary<string, (WebSocket Socket, TaskCompletionSource<object> Tcs)> _sockets = new();

        public static void AddSocket(WebSocket socket, TaskCompletionSource<object> socketFinishedTcs)
        {
            var id = Guid.NewGuid().ToString();
            _sockets.TryAdd(id, (socket, socketFinishedTcs));

                _ = Task.Run(() => ReceberMensagemAsync(id, socket, socketFinishedTcs));
        }

        private static async Task ReceberMensagemAsync(string id, WebSocket socket, TaskCompletionSource<object> tcs)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechado pelo cliente", CancellationToken.None);
                        break;
                    }

                    var mensagem = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // exemplo: faz broadcast da mensagem recebida para todos os conectados
                    await BroadcastAsync($"[{id[..4]}]: {mensagem}");
                }
            }
            catch (WebSocketException)
            {
                // cliente desconectou abruptamente
            }
            finally
            {
                // remove o socket da lista e libera a requisição HTTP no WebSocketConfig
                _sockets.TryRemove(id, out _);
                tcs.TrySetResult(null!);
                socket.Dispose();
            }
        }

        public static async Task BroadcastAsync(string mensagem)
        {
            var bytes = Encoding.UTF8.GetBytes(mensagem);
            var segment = new ArraySegment<byte>(bytes);

            var socketsAbertos = _sockets.Values
                .Select(item => item.Socket)
                .Where(socket => socket.State == WebSocketState.Open);

            foreach (var socket in socketsAbertos)
            {
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
