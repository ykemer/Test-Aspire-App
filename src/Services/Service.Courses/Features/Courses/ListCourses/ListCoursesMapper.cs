using CoursesGRPC;

using Library.GRPC;

namespace Service.Courses.Features.Courses.ListCourses;

public static class ListCoursesMapper
{
  public static ListCoursesQuery ToListCoursesQuery(this GrpcListCoursesRequest request) =>
    new()
    {
      PageSize = request.PageSize,
      PageNumber = request.Page,
      Query = request.Query,
      EnrolledClasses = GrpcInput.ParseGuidList(request.EnrolledClasses, nameof(request.EnrolledClasses)),
      ShowAll = request.ShowAll
    };
}
