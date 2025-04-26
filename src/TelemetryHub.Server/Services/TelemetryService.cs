using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using TelemetryHub.Server;   

namespace TelemetryHub.Server.Services
{
  public class TelemetryService : Telemetry.TelemetryBase
  {
    public override Task<Empty> SendEvent(Event request, ServerCallContext context)
    {
      Console.WriteLine($"[Telemetry] {request.Service} → {request.Level}: {request.Message}");
      return Task.FromResult(new Empty());
    }
  }
}
