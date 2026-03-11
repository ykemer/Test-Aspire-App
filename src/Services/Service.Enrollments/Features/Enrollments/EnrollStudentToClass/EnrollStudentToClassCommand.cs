namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommand : IRequest<ErrorOr<Created>>
{
  public required Guid CourseId { get; init; }

  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
}
