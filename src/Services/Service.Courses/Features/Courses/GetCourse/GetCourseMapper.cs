using CoursesGRPC;

using Library.GRPC;

namespace Service.Courses.Features.Courses.GetCourse;

public static class GetCourseMapper
{
  public static GetCourseQuery ToGetCourseQuery(this GrpcGetCourseRequest request) =>
    new(
      GrpcInput.ParseGuid(request.Id, nameof(request.Id)),
      GrpcInput.ParseGuidList(request.EnrolledClasses, nameof(request.EnrolledClasses)),
      request.ShowAll
    );
}
