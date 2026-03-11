using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public static class GetStudentEnrollmentsMapper
{
  public static GetStudentEnrollmentsQuery MapToGetStudentEnrollmentsQuery(
    this GrpcGetStudentEnrollmentsRequest request)
  {
    return new(Guid.Parse(request.StudentId), string.IsNullOrEmpty(request.CourseId) ? null : Guid.Parse(request.CourseId));
  }
}
