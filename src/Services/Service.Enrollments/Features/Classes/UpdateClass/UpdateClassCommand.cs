namespace Service.Enrollments.Features.Classes.UpdateClass;

public sealed class UpdateClassCommand : IRequest<ErrorOr<Updated>>
{
  public required string Id { get; init; }
  public required string CourseId { get; init; }

  public required int MaxStudents { get; init; }

  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }
}
