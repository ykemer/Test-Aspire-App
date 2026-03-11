namespace Contracts.Classes.Commands;

public class UpdateClassCommand
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required DateTime RegistrationDeadline { get; set; } = DateTime.UtcNow;
  public required DateTime CourseStartDate { get; set; } = DateTime.UtcNow;
  public required DateTime CourseEndDate { get; set; } = DateTime.UtcNow;
  public required int MaxStudents { get; set; } = 0;
  public string UserId { get; init; }
}
