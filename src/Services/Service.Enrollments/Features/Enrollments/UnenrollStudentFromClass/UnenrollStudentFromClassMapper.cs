using Contracts.Enrollments.Commands;

using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public static class UnenrollStudentFromClassMapper
{
  public static UnenrollStudentFromClassCommand
    MapToUnenrollStudentFromClassCommand(this DeleteEnrollmentCommand command) => new()
  {
    ClassId = command.ClassId, CourseId = command.CourseId, StudentId = command.StudentId
  };
}
