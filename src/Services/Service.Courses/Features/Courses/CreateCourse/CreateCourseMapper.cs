using CoursesGRPC;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public static class CreateCourseMapper
{
  public static CreateCourseCommand MapToCreateCourseCommand(this GrpcCreateCourseRequest request) =>
    new(request.Name, request.Description);

  public static Course MapToCourse(this CreateCourseCommand request) => new()
  {
    Description = request.Description, Name = request.Name
  };
}
