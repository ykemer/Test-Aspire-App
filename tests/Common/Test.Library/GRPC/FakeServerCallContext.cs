using Grpc.Core;

namespace Test.Library.GRPC;

/// <summary>
/// The smallest possible gRPC call context: only the request headers matter for these tests.
/// </summary>
public class FakeServerCallContext : ServerCallContext
{
  private readonly Metadata _requestHeaders;

  public FakeServerCallContext(Metadata requestHeaders) => _requestHeaders = requestHeaders;

  protected override Metadata RequestHeadersCore => _requestHeaders;
  protected override string MethodCore => "/test/Method";
  protected override string HostCore => "localhost";
  protected override string PeerCore => "ipv4:127.0.0.1";
  protected override DateTime DeadlineCore => DateTime.MaxValue;
  protected override CancellationToken CancellationTokenCore => CancellationToken.None;
  protected override Metadata ResponseTrailersCore { get; } = [];
  protected override Status StatusCore { get; set; }
  protected override WriteOptions? WriteOptionsCore { get; set; }
  protected override AuthContext AuthContextCore => new(null, new Dictionary<string, List<AuthProperty>>());

  protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) =>
    throw new NotSupportedException();

  protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
}
