using Domain.Enums;
using Domain.ValueObjects;
using Infra.Sockets;

namespace Adapter.InBound.WebSocketAdapter.Ports
{
    public static class WebSocketEndpoint
    {
        public static IEndpointRouteBuilder MapWebSocketEndpoint(
            this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/ws")
                .WithTags("WebSockets");

            // cast (Delegate) força o compilador a escolher a sobrecarga que retorna RouteHandlerBuilder
            group.MapGet("/", (Delegate)ConectarWebSocketAsync)
                .WithName("ConectarWebSocket")
                .Produces(StatusCodes.Status101SwitchingProtocols)
                .Produces<BaseError>(StatusCodes.Status400BadRequest);

            return endpoints;
        }

        private static async Task<IResult> ConectarWebSocketAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return Results.Json(
                    new BaseError(
                        EnumStatus.NEGOCIO,
                        1000,
                        "A requisição deve ser uma conexão WebSocket."),
                    statusCode: StatusCodes.Status400BadRequest);
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();

            BackgroundSocketProcessor.AddSocket(webSocket, socketFinishedTcs);

            await socketFinishedTcs.Task;

            return Results.Empty;
        }
    }
}
