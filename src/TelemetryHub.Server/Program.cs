using Microsoft.AspNetCore.Server.Kestrel.Core;
using TelemetryHub.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

//builder.WebHost.ConfigureKestrel(k =>
//{
//  k.ListenLocalhost(5121, lo => lo.Protocols = HttpProtocols.Http2);
//});

var app = builder.Build();
app.MapGrpcService<TelemetryService>();
app.MapGet("/", () => "TelemetryHub gRPC Server");
app.Run();
