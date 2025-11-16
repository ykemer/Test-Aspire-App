using CoursesGRPC;

namespace Service.Courses.Features.Courses.GetCourse;

public static class GetCourseMapper
{
  public static GetCourseQuery ToGetCourseQuery(this GrpcGetCourseRequest request) =>
    new(request.Id, [.. request.EnrolledClasses], request.ShowAll);

}
