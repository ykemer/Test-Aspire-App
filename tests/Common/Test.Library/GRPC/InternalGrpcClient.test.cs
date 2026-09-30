using System.Text;

using Grpc.Core;

using Library.GRPC.InternalApi;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Library.GRPC;

/// <summary>
/// Proves that a client registered with AddInternalGrpcClient really puts the key on the outgoing HTTP request.
/// </summary>
[TestFixture]
public class InternalGrpcClientTests
{
  private const string Key = "a-very-long-and-random-internal-api-key-0123456789";

  [Test]
  public async Task Calls_CarryTheInternalApiKeyHeader_OverHttps()
  {
    // Arrange: a client whose network layer records the request instead of sending it
    var recorder = new RecordingHandler();
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?> { ["InternalApi:Key"] = Key })
      .Build());
    services.AddInternalApiKey();
    services.AddInternalGrpcClient<PingClient>("pingService")
      .ConfigurePrimaryHttpMessageHandler(() => recorder);

    await using var provider = services.BuildServiceProvider();
    var client = provider.GetRequiredService<PingClient>();

    // Act (the fake network always fails, we only care about what was sent)
    Assert.ThrowsAsync<RpcException>(() => client.PingAsync());

    // Assert
    var request = recorder.LastRequest!;
    Assert.That(request.RequestUri!.Scheme, Is.EqualTo("https"));
    Assert.That(request.RequestUri.Host, Is.EqualTo("pingservice"));
    Assert.That(request.Headers.GetValues(InternalApiOptions.HeaderName), Is.EqualTo(new[] { Key }));
  }

  /// <summary>A tiny hand-written gRPC client with one call, standing in for the generated ones.</summary>
  public class PingClient : ClientBase<PingClient>
  {
    private static readonly Marshaller<string> s_text =
      Marshallers.Create(Encoding.UTF8.GetBytes, bytes => Encoding.UTF8.GetString(bytes));

    private static readonly Method<string, string> s_ping = new(MethodType.Unary, "test.Ping", "Ping", s_text, s_text);

    public PingClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }

    private PingClient(ClientBaseConfiguration configuration) : base(configuration)
    {
    }

    public async Task<string> PingAsync() => await CallInvoker.AsyncUnaryCall(s_ping, null, new CallOptions(), "ping");

    protected override PingClient NewInstance(ClientBaseConfiguration configuration) => new(configuration);
  }

  private class RecordingHandler : HttpMessageHandler
  {
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      LastRequest = request;
      throw new HttpRequestException("Fake network: nothing is really sent.");
    }
  }
}
