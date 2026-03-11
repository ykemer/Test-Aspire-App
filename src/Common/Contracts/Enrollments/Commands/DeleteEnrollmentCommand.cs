namespace Contracts.Enrollments.Commands;

public class DeleteEnrollmentCommand
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
}
