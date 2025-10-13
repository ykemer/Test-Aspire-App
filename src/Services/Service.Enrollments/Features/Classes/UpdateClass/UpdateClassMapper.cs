using Contracts.Courses.Events;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static void AddCommandData(this Class entity, UpdateClassCommand command)
  {
    entity.MaxStudents = command.MaxStudents;
    entity.RegistrationDeadline = command.RegistrationDeadline;
    entity.CourseStartDate = command.CourseStartDate;
    entity.CourseEndDate = command.CourseEndDate;
  }

  public static UpdateClassCommand MapToCreateClassCommand(this ClassUpdatedEvent updatedEvent) =>
    new()
    {
      Id = updatedEvent.Id,
      CourseId = updatedEvent.CourseId,
      MaxStudents = updatedEvent.MaxStudents,
      RegistrationDeadline = updatedEvent.RegistrationDeadline,
      CourseStartDate = updatedEvent.CourseStartDate,
      CourseEndDate = updatedEvent.CourseEndDate
    };
}
