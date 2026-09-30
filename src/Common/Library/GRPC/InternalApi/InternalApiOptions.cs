using System.ComponentModel.DataAnnotations;

namespace Library.GRPC.InternalApi;

/// <summary>
/// The shared secret that lets our own services call each other's internal gRPC APIs.
/// The Aspire AppHost generates it on every run and passes it as the "InternalApi__Key" environment variable.
/// </summary>
public class InternalApiOptions
{
  public const string SectionName = "InternalApi";
  public const string HeaderName = "x-internal-api-key";
  public const int MinKeyLength = 32;

  [Required(ErrorMessage = "InternalApi:Key is missing. It is normally provided by the Aspire AppHost.")]
  [MinLength(MinKeyLength, ErrorMessage = "InternalApi:Key must be at least 32 characters long.")]
  public string Key { get; set; } = string.Empty;
}
