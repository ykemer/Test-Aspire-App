namespace Contracts.Classes.Events;

public record ClassCreatedEvent
{
  public required Guid Id { get; init; }
  public required Guid CourseId { get; init; }

  public required int MaxStudents { get; init; }

  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }

  public required string UserId { get; init; }
}
