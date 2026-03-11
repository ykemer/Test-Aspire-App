using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public static class GetClassEnrollmentsMapper
{
  public static GetClassEnrollmentsQuery MapToGetClassEnrollmentsQuery(this GrpcGetClassEnrollmentsRequest request) =>
    new(Guid.Parse(request.CourseId), Guid.Parse(request.ClassId));
}
