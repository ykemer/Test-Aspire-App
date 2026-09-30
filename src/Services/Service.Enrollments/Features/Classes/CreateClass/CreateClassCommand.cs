namespace Service.Enrollments.Features.Classes.CreateClass;

public sealed record CreateClassCommand : IRequest<ErrorOr<Created>>, IClassDetails
{
  public required Guid Id { get; init; }
  public required Guid CourseId { get; init; }
  public required int MaxStudents { get; init; }
  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }
}
