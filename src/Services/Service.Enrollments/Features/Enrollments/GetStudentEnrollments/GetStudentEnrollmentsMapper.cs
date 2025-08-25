using EnrollmentsGRPC;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public static class GetStudentEnrollmentsMapper
{
  public static GetStudentEnrollmentsQuery MapToGetStudentEnrollmentsQuery(
    this GrpcGetStudentEnrollmentsRequest request) => new(request.StudentId, request.CourseId);
}
