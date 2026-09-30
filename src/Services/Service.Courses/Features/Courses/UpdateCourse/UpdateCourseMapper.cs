using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.UpdateCourse;

public static class UpdateCourseMapper
{
  public static void ApplyUpdate(this Course course, UpdateCourseCommand command, DateTime now)
  {
    course.Name = command.Name;
    course.Description = command.Description;
    course.UpdatedAt = now;
  }
}
