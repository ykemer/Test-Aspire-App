namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommand : IRequest<ErrorOr<Deleted>>
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
}
