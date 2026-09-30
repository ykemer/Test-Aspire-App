using ErrorOr;

using Grpc.Core;

namespace Library.GRPC;

/// <summary>
/// A "conflict" can mean different things. These helpers record which one it is,
/// so <see cref="GrpcErrorHandler"/> can send the right gRPC status to the caller.
/// A plain <c>Error.Conflict(...)</c> is treated as <see cref="StatePreventsAction"/>.
/// </summary>
public static class ConflictErrors
{
  public const string GrpcStatusMetadataKey = "grpcStatus";

  /// <summary>The thing the caller tries to create already exists (e.g. a duplicate name).</summary>
  public static Error AlreadyExists(string code, string description) =>
    Create(code, description, StatusCode.AlreadyExists);

  /// <summary>Someone else changed the same data at the same time. Reloading and retrying may work.</summary>
  public static Error ConcurrentUpdate(string code, string description) =>
    Create(code, description, StatusCode.Aborted);

  /// <summary>The current state does not allow the action (e.g. class is full, has students).</summary>
  public static Error StatePreventsAction(string code, string description) =>
    Create(code, description, StatusCode.FailedPrecondition);

  private static Error Create(string code, string description, StatusCode status) =>
    Error.Conflict(code, description, new Dictionary<string, object> { [GrpcStatusMetadataKey] = status });
}
