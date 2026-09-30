using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public static class CreateCourseMapper
{
  public static Course ToCourse(this CreateCourseCommand command, DateTime now) =>
    new()
    {
      Name = command.Name,
      Description = command.Description,
      CreatedAt = now,
      UpdatedAt = now
    };
}
