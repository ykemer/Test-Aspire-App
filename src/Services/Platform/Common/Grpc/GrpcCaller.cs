using Grpc.Core;

namespace Platform.Common.Grpc;

public interface IGrpcCaller
{
  /// <summary>Waits for a gRPC call and turns a gRPC failure into a normal error result.</summary>
  Task<ErrorOr<TResponse>> CallAsync<TResponse>(AsyncUnaryCall<TResponse> call);
}

/// <summary>
/// Calls our internal gRPC services and translates their status codes into errors the API can return.
/// </summary>
public sealed class GrpcCaller : IGrpcCaller
{
  private readonly ILogger<GrpcCaller> _logger;

  public GrpcCaller(ILogger<GrpcCaller> logger) => _logger = logger;

  public async Task<ErrorOr<TResponse>> CallAsync<TResponse>(AsyncUnaryCall<TResponse> call)
  {
    try
    {
      return await call.ResponseAsync;
    }
    catch (RpcException exception)
    {
      var error = ToError(exception.Status);
      var level = error.Type is ErrorType.Unexpected or ErrorType.Failure ? LogLevel.Error : LogLevel.Warning;
      _logger.Log(level, exception, "gRPC call failed with {StatusCode}", exception.StatusCode);
      return error;
    }
  }

  public static Error ToError(Status status) =>
    status.StatusCode switch
    {
      // All three are "conflict" errors in the downstream service (see Library.GRPC.ConflictErrors).
      StatusCode.AlreadyExists or StatusCode.FailedPrecondition or StatusCode.Aborted =>
        Error.Conflict(description: status.Detail),
      StatusCode.NotFound => Error.NotFound(description: status.Detail),
      StatusCode.InvalidArgument => Error.Validation(description: status.Detail),
      StatusCode.PermissionDenied => Error.Forbidden(description: status.Detail),

      // The downstream service is down or too slow: the user can try again later (HTTP 503).
      StatusCode.Unavailable or StatusCode.DeadlineExceeded =>
        Error.Failure(description: "A required service is temporarily unavailable. Please try again."),

      // "Unauthenticated" here means OUR internal API key was rejected: a server configuration problem.
      // It must not become HTTP 401, which would tell the user that their own session expired.
      _ => Error.Unexpected(description: "An unexpected error occurred.")
    };
}
