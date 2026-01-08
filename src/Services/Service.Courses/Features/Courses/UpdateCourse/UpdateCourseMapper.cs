using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.UpdateCourse;

public static class UpdateClassMapper
{
  public static void AddCommandValues(this Course entity, UpdateCourseCommand command)
  {
    entity.Name = command.Name;
    entity.Description = command.Description;
  }
}
