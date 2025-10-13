using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommand : IRequest<ErrorOr<Class>>
{
  public required string CourseId { get; init; }
  public required DateTime RegistrationDeadline { get; init;}
  public required DateTime CourseStartDate { get; init;}
  public required DateTime CourseEndDate { get; init;}
  public required int MaxStudents { get; set;}
}
