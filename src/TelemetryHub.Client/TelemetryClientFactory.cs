using Grpc.Net.Client;
using TelemetryHub.Server;
using Microsoft.Extensions.DependencyInjection; 
using TelemetryHub.Client;                         


namespace TelemetryHub.Client
{
  public static class TelemetryClientFactory
  {
    public static Server.Telemetry.TelemetryClient Create(string address)
    {
      var channel = GrpcChannel.ForAddress(address);
      return new Server.Telemetry.TelemetryClient(channel);
    }
  }
}
