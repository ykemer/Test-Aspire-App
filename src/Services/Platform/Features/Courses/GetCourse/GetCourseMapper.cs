using Contracts.Courses.Responses;

using CoursesGRPCClient;

namespace Platform.Features.Courses.GetCourse;

public static class GetCourseMapper
{
  public static CourseResponse ToCourseResponse(this GrpcCourseResponse course) =>
    new()
    {
      Id = Guid.Parse(course.Id),
      Description = course.Description,
      Name = course.Name,
      TotalStudents = course.TotalStudents
    };
}
