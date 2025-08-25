using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public static class GetClassEnrollmentsMapper
{
  public static GetClassEnrollmentsQuery MapToGetClassEnrollmentsQuery(this GrpcGetClassEnrollmentsRequest request) => new(request.CourseId, request.ClassId);
}
