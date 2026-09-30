using Grpc.Core;

namespace Library.GRPC;

/// <summary>
/// Turns raw gRPC request fields into safe values.
/// Bad input becomes an "InvalidArgument" error for the caller instead of crashing the handler.
/// </summary>
public static class GrpcInput
{
  public const int MaxIdsPerRequest = 100;

  public static Guid ParseGuid(string value, string fieldName) =>
    Guid.TryParse(value, out var id)
      ? id
      : throw InvalidArgument($"'{fieldName}' must be a valid GUID.");

  public static List<Guid> ParseGuidList(IReadOnlyCollection<string> values, string fieldName)
  {
    if (values.Count > MaxIdsPerRequest)
    {
      throw InvalidArgument($"'{fieldName}' must not contain more than {MaxIdsPerRequest} items.");
    }

    return values.Select(value => ParseGuid(value, fieldName)).ToList();
  }

  private static RpcException InvalidArgument(string message) =>
    new(new Status(StatusCode.InvalidArgument, message));
}
