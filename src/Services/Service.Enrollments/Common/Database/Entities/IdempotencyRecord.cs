namespace Service.Enrollments.Common.Database.Entities;

/// <summary>
/// Remembers a client request key (one enroll or unenroll attempt) that was already handled,
/// so a repeated request returns success instead of doing the work twice.
/// </summary>
public class IdempotencyRecord
{
  public const string EnrollOperation = "Enroll";
  public const string UnenrollOperation = "Unenroll";

  public required Guid IdempotencyKey { get; init; }

  public required string Operation { get; init; }

  public required DateTime CreatedAt { get; init; }
}
