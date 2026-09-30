using Contracts.Classes.Events;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Classes.CreateClass;

public static class CreateClassMapper
{
  public static CreateClassCommand ToCreateClassCommand(this ClassCreatedEvent createdEvent) =>
    new()
    {
      Id = createdEvent.Id,
      CourseId = createdEvent.CourseId,
      MaxStudents = createdEvent.MaxStudents,
      RegistrationDeadline = createdEvent.RegistrationDeadline,
      CourseStartDate = createdEvent.CourseStartDate,
      CourseEndDate = createdEvent.CourseEndDate
    };

  public static Class ToClass(this CreateClassCommand command, DateTime now) =>
    new()
    {
      Id = command.Id,
      CourseId = command.CourseId,
      MaxStudents = command.MaxStudents,
      RegistrationDeadline = command.RegistrationDeadline,
      CourseStartDate = command.CourseStartDate,
      CourseEndDate = command.CourseEndDate,
      CreatedAt = now,
      UpdatedAt = now
    };
}
