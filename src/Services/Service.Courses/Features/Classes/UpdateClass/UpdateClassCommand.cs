namespace Service.Courses.Features.Classes.UpdateClass;

public record UpdateClassCommand : IRequest<ErrorOr<Updated>>
{
  public required Guid Id { get; init; }
  public required Guid CourseId { get; init; }
  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }
  public int MaxStudents { get; set; }
}
