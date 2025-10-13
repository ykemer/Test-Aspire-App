using ClassesGRPC;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static UpdateClassCommand MapToUpdateClassCommand(this GrpcUpdateClassRequest request) =>
    new()
    {
      Id = request.Id,
      CourseId = request.CourseId,
      RegistrationDeadline = request.RegistrationDeadline.ToDateTime(),
      CourseStartDate = request.CourseStartDate.ToDateTime(),
      CourseEndDate = request.CourseEndDate.ToDateTime(),
      MaxStudents = request.MaxStudents
    };

  public static void AddCommandValues(this Class entity, UpdateClassCommand command)
  {
    entity.CourseId = command.CourseId;
    entity.RegistrationDeadline = command.RegistrationDeadline;
    entity.CourseStartDate = command.CourseStartDate;
    entity.CourseEndDate = command.CourseEndDate;
    entity.MaxStudents = command.MaxStudents;
  }
}
