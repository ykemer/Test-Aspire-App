namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommand : IRequest<ErrorOr<Created>>
{
  public required string CourseId { get; init; }

  public required string ClassId { get; init; }
  public required string StudentId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
}
