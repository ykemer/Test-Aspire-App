using EnrollmentsGRPC;

using Library.GRPC;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public static class GetClassEnrollmentsMapper
{
  public static GetClassEnrollmentsQuery ToGetClassEnrollmentsQuery(this GrpcGetClassEnrollmentsRequest request) =>
    new(
      GrpcInput.ParseGuid(request.CourseId, nameof(request.CourseId)),
      GrpcInput.ParseGuid(request.ClassId, nameof(request.ClassId)));
}
