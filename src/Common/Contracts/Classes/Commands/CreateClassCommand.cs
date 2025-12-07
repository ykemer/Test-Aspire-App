namespace Contracts.Classes.Commands;

public class CreateClassCommand
{
  public required string CourseId { get; init; }
  public required DateTime RegistrationDeadline { get; set; } = DateTime.UtcNow;
  public required DateTime CourseStartDate { get; set; } = DateTime.UtcNow;
  public required DateTime CourseEndDate { get; set; } = DateTime.UtcNow;
  public required int MaxStudents { get; set; } = 0;
  public string UserId { get; init; }
}
