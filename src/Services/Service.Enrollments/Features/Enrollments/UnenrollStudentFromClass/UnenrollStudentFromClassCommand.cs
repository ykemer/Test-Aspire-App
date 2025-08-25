namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommand : IRequest<ErrorOr<Deleted>>
{
  public required string CourseId { get; init; }
  public required string ClassId { get; init; }
  public required string StudentId { get; init; }
}
