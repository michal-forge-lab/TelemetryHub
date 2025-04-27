using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using TelemetryHub.Shared;
using TelemetryHub.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddSingleton(_ =>
{
  var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
  var channel = GrpcChannel.ForAddress("http://localhost:5121",
      new GrpcChannelOptions { HttpHandler = handler });

  return new Telemetry.TelemetryClient(channel); 
});

await builder.Build().RunAsync();
