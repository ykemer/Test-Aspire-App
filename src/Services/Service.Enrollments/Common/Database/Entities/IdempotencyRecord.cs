namespace Service.Enrollments.Common.Database.Entities;

public class IdempotencyRecord
{
  public required Guid IdempotencyKey { get; init; }

  public required string Operation { get; init; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
