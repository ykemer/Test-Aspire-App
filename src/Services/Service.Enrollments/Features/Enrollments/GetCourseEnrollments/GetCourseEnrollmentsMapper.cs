using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public static class GetCourseEnrollmentsMapper
{
  public static GetCourseEnrollmentsQuery
    MapToGetCourseEnrollmentsQuery(this GrpcGetCourseEnrollmentsRequest request) => new(request.CourseId);
}
