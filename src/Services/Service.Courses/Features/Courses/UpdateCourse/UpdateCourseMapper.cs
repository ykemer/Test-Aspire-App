using CoursesGRPC;

using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Courses.UpdateCourse;

public static class UpdateClassMapper
{
  public static UpdateCourseCommand MapToUpdateCourseCommand(this GrpcUpdateCourseRequest request) =>
    new() { Id = request.Id, Name = request.Name, Description = request.Description };

  public static void AddCommandValues(this Course entity, UpdateCourseCommand command)
  {
    entity.Name = command.Name;
    entity.Description = command.Description;
  }
}
