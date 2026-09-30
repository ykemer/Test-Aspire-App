using EnrollmentsGRPC;

using Library.GRPC;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public static class GetCourseEnrollmentsMapper
{
  public static GetCourseEnrollmentsQuery ToGetCourseEnrollmentsQuery(this GrpcGetCourseEnrollmentsRequest request) =>
    new(GrpcInput.ParseGuid(request.CourseId, nameof(request.CourseId)));
}
