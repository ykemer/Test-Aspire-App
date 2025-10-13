namespace Contracts.Enrollments.Commands;

public class DeleteEnrollmentCommand
{
  public required string CourseId { get; init; }
  public required string ClassId { get; init; }
  public required string StudentId { get; init; }
}
