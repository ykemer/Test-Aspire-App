using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public static class EnrollStudentToClassMapper
{
  public static EnrollStudentToClassCommand MapToEnrollStudentToClassCommand(this GrpcEnrollStudentRequest command) =>
    new()
    {
      ClassId = command.ClassId,
      CourseId = command.CourseId,
      StudentId = command.StudentId,
      FirstName = command.StudentFirstName,
      LastName = command.StudentLastName
    };
}
