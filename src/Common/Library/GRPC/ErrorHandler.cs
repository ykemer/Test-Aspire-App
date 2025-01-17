using ErrorOr;

using Grpc.Core;

using Microsoft.Extensions.Logging;

namespace Library.GRPC;

public static class GrpcErrorHandler
{
  public static RpcException ThrowAndLogRpcException(List<Error> errors, ILogger logger)
  {
    var firstError = errors[0];
    var status = GetErrorStatus(firstError);

    var message = string.Join(", ", errors.Select(i => i.Description));
    LogError(message, logger);
    return new RpcException(new Status(status, message));
  }

  private static void LogError(string message, ILogger logger) => logger.LogError(message);

  private static StatusCode GetErrorStatus(Error error) =>
    error switch
    {
      { Type: ErrorType.NotFound } => StatusCode.NotFound,
      { Type: ErrorType.Conflict } => StatusCode.AlreadyExists,
      { Type: ErrorType.Validation } => StatusCode.InvalidArgument,
      { Type: ErrorType.Forbidden } => StatusCode.PermissionDenied,
      { Type: ErrorType.Unauthorized } => StatusCode.Unauthenticated,
      _ => StatusCode.Internal
    };
}
