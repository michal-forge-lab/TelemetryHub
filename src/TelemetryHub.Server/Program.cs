using TelemetryHub.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<TelemetryService>();
app.MapGet("/", () => "TelemetryHub gRPC Server");
app.Run();
