namespace Contracts.Courses.Requests.Classes;

public class CreateClassRequest
{
  public required DateTime RegistrationDeadline { get; set; } = DateTime.UtcNow;
  public required DateTime CourseStartDate { get; set; } = DateTime.UtcNow;
  public required DateTime CourseEndDate { get; set; } = DateTime.UtcNow;
  public required int MaxStudents { get; set; } = 0;
}
