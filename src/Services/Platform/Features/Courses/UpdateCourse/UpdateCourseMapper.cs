using Contracts.Courses.Requests;

using CoursesGRPCClient;

namespace Platform.Features.Courses.UpdateCourse;

public static class UpdateCourseMapper
{
  public static GrpcUpdateCourseRequest ToGrpcUpdateCourseRequest(this UpdateCourseRequest updateCourseCommand, string CourseId) =>
    new()
    {
      Id = CourseId, Name = updateCourseCommand.Name, Description = updateCourseCommand.Description
    };
}
