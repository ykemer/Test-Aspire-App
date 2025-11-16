namespace Service.Courses.Features.Classes.UpdateClass;

public record UpdateClassCommand : IRequest<ErrorOr<Updated>>
{
  public required string Id { get; init; }
  public required string CourseId { get; init; }
  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }
  public int MaxStudents { get; set; }
}
