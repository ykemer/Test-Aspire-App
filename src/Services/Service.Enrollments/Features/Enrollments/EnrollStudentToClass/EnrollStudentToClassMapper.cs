using Contracts.Enrollments.Commands;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public static class EnrollStudentToClassMapper
{
  public static EnrollStudentToClassCommand MapToEnrollStudentToClassCommand(this CreateEnrollmentCommand command) =>
    new()
    {
      ClassId = command.ClassId,
      CourseId = command.CourseId,
      StudentId = command.StudentId,
      FirstName = command.FirstName,
      LastName = command.LastName
    };
}
