using Grpc.Core;

namespace Platform.Middleware.Grpc;

public class GrpcRequestMiddleware : IGrpcRequestMiddleware
{
  private readonly ILogger<GrpcRequestMiddleware> _logger;

  public GrpcRequestMiddleware(ILogger<GrpcRequestMiddleware> logger) => _logger = logger;

  public async Task<ErrorOr<TResponse>> SendGrpcRequestAsync<TResponse>(AsyncUnaryCall<TResponse> grpcCall,
    CancellationToken ct)
  {
    try
    {
      TResponse? response = await grpcCall.ResponseAsync;
      return response;
    }
    catch (RpcException e)
    {
      _logger.LogError(e, "Error while sending gRPC request");
      return e.Status.StatusCode switch
      {
        StatusCode.AlreadyExists => Error.Conflict(description: e.Status.Detail),
        StatusCode.NotFound => Error.NotFound(description: e.Status.Detail),
        StatusCode.InvalidArgument => Error.Validation(description: e.Status.Detail),
        StatusCode.PermissionDenied => Error.Forbidden(description: e.Status.Detail),
        StatusCode.Unauthenticated => Error.Unauthorized(description: e.Status.Detail),
        _ => Error.Unexpected(description: e.Status.Detail)
      };
    }
  }
}
