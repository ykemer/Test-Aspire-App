using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.CreateClass;

public static class CreateClassMapper
{
  public static Class MapToClass(this CreateClassCommand command) =>
    new()
    {
      CourseId = command.CourseId,
      RegistrationDeadline = command.RegistrationDeadline,
      CourseStartDate = command.CourseStartDate,
      CourseEndDate = command.CourseEndDate,
      MaxStudents = command.MaxStudents
    };
}
