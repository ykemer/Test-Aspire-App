using Contracts.Enrollments.Commands;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public static class EnrollStudentToClassMapper
{
  public static EnrollStudentToClassCommand ToEnrollStudentToClassCommand(this CreateEnrollmentCommand command) =>
    new()
    {
      ClassId = command.ClassId,
      CourseId = command.CourseId,
      StudentId = command.StudentId,
      FirstName = command.FirstName,
      LastName = command.LastName,
      IdempotencyKey = command.IdempotencyKey
    };

  public static Enrollment ToEnrollment(this EnrollStudentToClassCommand command, DateTime now) =>
    new()
    {
      CourseId = command.CourseId,
      ClassId = command.ClassId,
      StudentId = command.StudentId,
      StudentFirstName = command.FirstName,
      StudentLastName = command.LastName,
      EnrollmentDateTime = now,
      CreatedAt = now,
      UpdatedAt = now
    };
}
