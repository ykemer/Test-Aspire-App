namespace Contracts.Classes.Responses;

public class ClassResponse
{
  public required Guid Id { get; init; }
  public required Guid CourseId { get; init; }

  public required DateTime RegistrationDeadline { get; set; }
  public required DateTime CourseStartDate { get; set; }
  public required DateTime CourseEndDate { get; set; }
  public required int MaxStudents { get; set; } = 0;
  public required int TotalStudents { get; set; } = 0;
}
