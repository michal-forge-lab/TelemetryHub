using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using TelemetryHub.Server;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

class Program
{
  static async Task Main()
  {
    AppContext.SetSwitch(
        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

    var handler = new SocketsHttpHandler { EnableMultipleHttp2Connections = true };

    using var channel = GrpcChannel.ForAddress(
        "https://localhost:7033",
        new GrpcChannelOptions { HttpHandler = handler });

    var client = new Telemetry.TelemetryClient(channel);

    using var call = client.Subscribe(new Empty());
    Console.WriteLine("Subscribed over HTTP/2 plaintext. Ctrl+C to quit.");

    while (await call.ResponseStream.MoveNext())
    {
      var ev = call.ResponseStream.Current;
      Console.WriteLine($"{ev.Timestamp} [{ev.Level}] {ev.Service}: {ev.Message}");
    }
  }
}
