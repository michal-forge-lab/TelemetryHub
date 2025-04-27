// src/TelemetryHub.Server/Services/TelemetryService.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace TelemetryHub.Server.Services
{
  public class TelemetryService : Telemetry.TelemetryBase
  {
    private readonly List<IServerStreamWriter<Event>> _subscribers = new();

    /// <summary>
    /// Server‐streaming RPC: clients subscribe here to receive all future events.
    /// </summary>
    public override async Task Subscribe(
        Empty _,
        IServerStreamWriter<Event> responseStream,
        ServerCallContext ctx)
    {
      lock (_subscribers)
      {
        _subscribers.Add(responseStream);
      }

      try
      {
        while (!ctx.CancellationToken.IsCancellationRequested)
        {
          await Task.Delay(1_000, ctx.CancellationToken);
        }
      }
      catch (TaskCanceledException)
      {

      }
      finally
      {
        lock (_subscribers)
        {
          _subscribers.Remove(responseStream);
        }
      }
    }

    /// <summary>
    /// Unary RPC: send a single event to the server (and fan-out to subscribers).
    /// </summary>
    public override Task<Empty> SendEvent(Event request, ServerCallContext ctx)
    {
      lock (_subscribers)
      {
        foreach (var sub in _subscribers)
        {
          _ = sub.WriteAsync(request);
        }
      }

      Console.WriteLine($"[Telemetry] {request.Service}: {request.Message}");

      return Task.FromResult(new Empty());
    }

    /// <summary>
    /// Client‐streaming RPC: client sends a batch of events, server responds once when done.
    /// </summary>
    public override async Task<Empty> BulkUpload(
        IAsyncStreamReader<Event> requestStream,
        ServerCallContext ctx)
    {
      await foreach (var ev in requestStream.ReadAllAsync())
      {
        Console.WriteLine($"[Bulk] {ev.Service}: {ev.Message}");
      }

      return new Empty();
    }

    // Optionally, you could add LiveFeed for bidirectional streaming:
    /*
    public override async Task LiveFeed(
        IAsyncStreamReader<Event> requestStream,
        IServerStreamWriter<Event> responseStream,
        ServerCallContext ctx)
    {
        await foreach (var ev in requestStream.ReadAllAsync())
        {
            // Echo back each event
            await responseStream.WriteAsync(ev);
        }
    }
    */
  }
}
