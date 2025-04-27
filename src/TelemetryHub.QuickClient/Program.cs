
using Grpc.Net.Client;
using TelemetryHub.Server;          
using Google.Protobuf.WellKnownTypes;

class Program
{
  static async Task Main()
  {
    // Allow HTTP/2 without TLS (plaintext) for local development
    AppContext.SetSwitch(
        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
        true);

    // Create a handler that supports multiple HTTP/2 connections
    var httpHandler = new SocketsHttpHandler
    {
      EnableMultipleHttp2Connections = true
    };

    // Build a gRPC channel targeting our server's plaintext HTTP/2 endpoint
    using var channel = GrpcChannel.ForAddress(
        "http://localhost:5121",
        new GrpcChannelOptions { HttpHandler = httpHandler });

    // Instantiate the generated Telemetry client
    var client = new Telemetry.TelemetryClient(channel);

    // ─────────── Send a single event ───────────
    // Create and send one Event message to the server
    var singleEvent = new Event
    {
      Service = "QuickClient",
      Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
      Level = "INFO",
      Message = "Hello from QuickClient!"
    };
    await client.SendEventAsync(singleEvent);
    Console.WriteLine("✔ Sent a single event.");

    // ─────────── Bulk upload example ───────────
    // Uncomment the following block to send a batch of events in one stream
    /*
    var batch = new[]
    {
        new Event { Service="QuickClient", Timestamp=1, Level="INFO",  Message="Bulk #1" },
        new Event { Service="QuickClient", Timestamp=2, Level="WARN",  Message="Bulk #2" },
        new Event { Service="QuickClient", Timestamp=3, Level="ERROR", Message="Bulk #3" }
    };

    // Start a client-streaming call
    using var bulkCall = client.BulkUpload();

    // Write each event into the request stream
    foreach (var ev in batch)
    {
        await bulkCall.RequestStream.WriteAsync(ev);
    }

    // Signal end-of-stream and wait for the server's Empty response
    await bulkCall.RequestStream.CompleteAsync();
    await bulkCall.ResponseAsync;
    Console.WriteLine("✔ Bulk upload complete.");
    */

    // ─────────── Subscribe to live feed ───────────
    // Open a server-streaming call to receive events as they arrive
    using var subscribeCall = client.Subscribe(new Empty());
    Console.WriteLine("Subscribed — streaming live events. Press Ctrl+C to exit.");

    // Continuously read from the response stream
    var responseStream = subscribeCall.ResponseStream;
    while (await responseStream.MoveNext(CancellationToken.None))
    {
      var ev = responseStream.Current;
      Console.WriteLine(
          $"{ev.Timestamp} [{ev.Level}] {ev.Service}: {ev.Message}");
    }
  }
}
