using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.CreateClass;

public record CreateClassCommand : IRequest<ErrorOr<Class>>, IClassDetails
{
  public required Guid CourseId { get; init; }
  public required DateTime RegistrationDeadline { get; init; }
  public required DateTime CourseStartDate { get; init; }
  public required DateTime CourseEndDate { get; init; }
  public required int MaxStudents { get; init; }
}
