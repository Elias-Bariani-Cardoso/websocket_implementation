using Adapter.InBound.WebSocketAdapter.Ports;
using Infra.Sockets;

namespace Infra.Configuration.InBound
{
    public static class WebSocketConfig
    {
        public static WebApplication AddWebSocketConfig(this WebApplication app)
        {
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            });

            app.MapWebSocketEndpoint();

            return app;
        }

        public static IServiceCollection AddWebSocketServices(
            this IServiceCollection services)
        {
            services.AddHostedService<BackgroundSocketProcessor>();

            return services;
        }
    }
}
