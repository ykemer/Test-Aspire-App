using Contracts.Classes.Events;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static UpdateClassCommand ToUpdateClassCommand(this ClassUpdatedEvent updatedEvent) =>
    new()
    {
      Id = updatedEvent.Id,
      CourseId = updatedEvent.CourseId,
      MaxStudents = updatedEvent.MaxStudents,
      RegistrationDeadline = updatedEvent.RegistrationDeadline,
      CourseStartDate = updatedEvent.CourseStartDate,
      CourseEndDate = updatedEvent.CourseEndDate
    };

  public static void ApplyUpdate(this Class courseClass, UpdateClassCommand command, DateTime now)
  {
    courseClass.MaxStudents = command.MaxStudents;
    courseClass.RegistrationDeadline = command.RegistrationDeadline;
    courseClass.CourseStartDate = command.CourseStartDate;
    courseClass.CourseEndDate = command.CourseEndDate;
    courseClass.UpdatedAt = now;
  }
}
