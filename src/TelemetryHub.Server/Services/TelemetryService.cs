using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using TelemetryHub.Server;   

namespace TelemetryHub.Server.Services
{
  public class TelemetryService : Telemetry.TelemetryBase
  {
    private readonly List<IServerStreamWriter<Event>> _subscribers = new();
    public override async Task Subscribe(
    Google.Protobuf.WellKnownTypes.Empty _,
    IServerStreamWriter<Event> responseStream,
    ServerCallContext ctx)
    {
      lock (_subscribers) _subscribers.Add(responseStream);

      // trzymamy połączenie otwarte
      while (!ctx.CancellationToken.IsCancellationRequested)
        await Task.Delay(1000);

      lock (_subscribers) _subscribers.Remove(responseStream);
    }
    public override Task<Empty> SendEvent(Event request, ServerCallContext ctx)
    {
      lock (_subscribers)
        foreach (var sub in _subscribers)
          _ = sub.WriteAsync(request);   // „fire-and-forget”

      Console.WriteLine($"[Telemetry] {request.Service}: {request.Message}");
      return Task.FromResult(new Empty());
    }
  }
}
