using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using TelemetryHub.Server.Data;
using TelemetryHub.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

//builder.WebHost.ConfigureKestrel(k =>
//{
//  k.ListenLocalhost(5121, lo => lo.Protocols = HttpProtocols.Http2);
//});
builder.Services.AddDbContext<TelemetryContext>(o =>
    o.UseSqlite("Data Source=telemetry.db"));

var app = builder.Build();
app.MapGrpcService<TelemetryService>();
app.MapGet("/", () => "TelemetryHub gRPC Server");
app.Run();
