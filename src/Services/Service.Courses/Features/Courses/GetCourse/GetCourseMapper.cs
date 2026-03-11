using CoursesGRPC;

namespace Service.Courses.Features.Courses.GetCourse;

public static class GetCourseMapper
{
  public static GetCourseQuery ToGetCourseQuery(this GrpcGetCourseRequest request) =>
    new(
      Guid.Parse(request.Id),
      request.EnrolledClasses.Select(Guid.Parse).ToList(),
      request.ShowAll
    );
}
