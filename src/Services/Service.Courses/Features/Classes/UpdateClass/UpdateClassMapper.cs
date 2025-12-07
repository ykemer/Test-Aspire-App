using ClassesGRPC;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static void AddCommandValues(this Class entity, UpdateClassCommand command)
  {
    entity.CourseId = command.CourseId;
    entity.RegistrationDeadline = command.RegistrationDeadline;
    entity.CourseStartDate = command.CourseStartDate;
    entity.CourseEndDate = command.CourseEndDate;
    entity.MaxStudents = command.MaxStudents;
  }
}
