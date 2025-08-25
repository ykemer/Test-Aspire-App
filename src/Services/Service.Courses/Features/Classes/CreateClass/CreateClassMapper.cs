using ClassesGRPC;

using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Classes.CreateClass;

public static class CreateClassMapper
{
  public static CreateClassCommand MapToCreateClassCommand(this GrpcCreateClassRequest request) =>
    new()
    {
      CourseId = request.CourseId,
      RegistrationDeadline = request.RegistrationDeadline.ToDateTime(),
      CourseStartDate = request.CourseStartDate.ToDateTime(),
      CourseEndDate = request.CourseEndDate.ToDateTime(),
      MaxStudents = request.MaxStudents
    };

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
