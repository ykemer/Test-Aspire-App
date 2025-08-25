namespace Contracts.Courses.Responses;

public class ClassListItemResponse
{
  public Guid Id { get; init; }
  public Guid CourseId { get; init; }

  public DateTime RegistrationDeadline { get; set; }
  public DateTime CourseStartDate { get; set; }
  public DateTime CourseEndDate { get; set; }
  public int MaxStudents { get; set; } = 0;

  public bool IsUserEnrolled { get; init; } = false;

  public int EnrollmentsCount { get; init; } = 0;
}
