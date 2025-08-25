namespace Contracts.Courses.Responses;

public class ClassResponse
{
  public required string Id { get; init; }
  public required string CourseId { get; init; }

  public required DateTime RegistrationDeadline { get; set; }
  public required DateTime CourseStartDate { get; set; }
  public required DateTime CourseEndDate { get; set; }
  public required int MaxStudents { get; set; } = 0;
  public required int TotalStudents { get; set; } = 0;
}
