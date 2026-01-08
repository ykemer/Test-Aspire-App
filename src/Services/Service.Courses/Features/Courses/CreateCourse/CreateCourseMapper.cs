using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public static class CreateCourseMapper
{
  public static Course MapToCourse(this CreateCourseCommand request) => new()
  {
    Description = request.Description, Name = request.Name
  };
}
