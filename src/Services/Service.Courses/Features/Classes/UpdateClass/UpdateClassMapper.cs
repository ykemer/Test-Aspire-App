using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static void ApplyUpdate(this Class courseClass, UpdateClassCommand command, DateTime now)
  {
    courseClass.RegistrationDeadline = command.RegistrationDeadline;
    courseClass.CourseStartDate = command.CourseStartDate;
    courseClass.CourseEndDate = command.CourseEndDate;
    courseClass.MaxStudents = command.MaxStudents;
    courseClass.UpdatedAt = now;
  }
}
