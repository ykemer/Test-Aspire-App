namespace Contracts.Enrollments.Commands;

public class CreateEnrollmentCommand
{
  public required string CourseId { get; init; }
  public required string ClassId { get; init; }
  public required string StudentId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
}
