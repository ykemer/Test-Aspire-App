namespace Contracts.Enrollments.Commands;

public class CreateEnrollmentCommand
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
}
