using ErrorOr;

using Grpc.Core;

using Microsoft.Extensions.Logging;

namespace Library.GRPC;

/// <summary>
/// Turns handler errors into a gRPC <see cref="RpcException"/> with the matching status code.
/// </summary>
public static class GrpcErrorHandler
{
  /// <summary>
  /// Text sent to callers for unexpected failures. The real details stay in our logs only.
  /// </summary>
  public const string InternalErrorMessage = "An unexpected error occurred.";

  public static RpcException ThrowAndLogRpcException(List<Error> errors, ILogger logger)
  {
    var status = GetStatusCode(errors[0]);
    var description = string.Join(", ", errors.Select(error => error.Description));
    var codes = string.Join(", ", errors.Select(error => error.Code));

    if (status == StatusCode.Internal)
    {
      logger.LogError("gRPC call failed unexpectedly. Codes: {ErrorCodes}. Details: {ErrorDescription}",
        codes, description);
      return new RpcException(new Status(StatusCode.Internal, InternalErrorMessage));
    }

    // Expected business outcomes (not found, invalid input, conflicts) are warnings, not errors.
    logger.LogWarning("gRPC call rejected with {StatusCode}. Codes: {ErrorCodes}. Details: {ErrorDescription}",
      status, codes, description);
    return new RpcException(new Status(status, description));
  }

  public static StatusCode GetStatusCode(Error error) =>
    error.Type switch
    {
      ErrorType.NotFound => StatusCode.NotFound,
      ErrorType.Conflict => GetConflictStatusCode(error),
      ErrorType.Validation => StatusCode.InvalidArgument,
      ErrorType.Forbidden => StatusCode.PermissionDenied,
      ErrorType.Unauthorized => StatusCode.Unauthenticated,
      _ => StatusCode.Internal
    };

  private static StatusCode GetConflictStatusCode(Error error) =>
    error.Metadata?.GetValueOrDefault(ConflictErrors.GrpcStatusMetadataKey) is StatusCode status
      ? status
      : StatusCode.FailedPrecondition;
}
