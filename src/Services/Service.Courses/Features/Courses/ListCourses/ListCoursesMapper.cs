using CoursesGRPC;

namespace Service.Courses.Features.Courses.ListCourses;

public static class ListCoursesMapper
{
  public static ListCoursesRequest MapToListCoursesRequest(this GrpcListCoursesRequest request) =>
    new()
    {
      PageSize = request.PageSize,
      PageNumber = request.Page,
      Query = request.Query,
      EnrolledClasses = [.. request.EnrolledClasses],
      ShowAll = request.ShowAll
    };
}
