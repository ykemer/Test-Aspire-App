using System.Security.Cryptography;
using System.Text;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.Extensions.Options;

namespace Library.GRPC.InternalApi;

/// <summary>
/// Runs before every gRPC call and rejects callers that do not send the correct internal API key.
/// </summary>
public class InternalApiKeyInterceptor : Interceptor
{
  private readonly byte[] _expectedKeyHash;

  public InternalApiKeyInterceptor(IOptions<InternalApiOptions> options) =>
    _expectedKeyHash = Hash(options.Value.Key);

  public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
    ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
  {
    EnsureCallerIsTrusted(context);
    return continuation(request, context);
  }

  public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
    IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
    ClientStreamingServerMethod<TRequest, TResponse> continuation)
  {
    EnsureCallerIsTrusted(context);
    return continuation(requestStream, context);
  }

  public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request,
    IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
    ServerStreamingServerMethod<TRequest, TResponse> continuation)
  {
    EnsureCallerIsTrusted(context);
    return continuation(request, responseStream, context);
  }

  public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
    IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
    DuplexStreamingServerMethod<TRequest, TResponse> continuation)
  {
    EnsureCallerIsTrusted(context);
    return continuation(requestStream, responseStream, context);
  }

  private void EnsureCallerIsTrusted(ServerCallContext context)
  {
    var sentKey = context.RequestHeaders.GetValue(InternalApiOptions.HeaderName) ?? string.Empty;

    // Compare hashes in constant time, so response timing does not reveal how much of the key was right.
    if (!CryptographicOperations.FixedTimeEquals(Hash(sentKey), _expectedKeyHash))
    {
      throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing or invalid internal API key."));
    }
  }

  private static byte[] Hash(string value) => SHA256.HashData(Encoding.UTF8.GetBytes(value));
}
