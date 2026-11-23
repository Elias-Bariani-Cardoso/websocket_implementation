using Infra.Configuration.InBound;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSocketServices();

var app = builder.Build();

app.AddWebSocketConfig();

await app.RunAsync();
