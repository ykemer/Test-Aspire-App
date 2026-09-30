using EnrollmentsGRPC;

using Library.GRPC;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public static class GetStudentEnrollmentsMapper
{
  public static GetStudentEnrollmentsQuery ToGetStudentEnrollmentsQuery(this GrpcGetStudentEnrollmentsRequest request)
  {
    var studentId = GrpcInput.ParseGuid(request.StudentId, nameof(request.StudentId));

    // The course filter is optional: empty means "all courses".
    Guid? courseId = string.IsNullOrEmpty(request.CourseId)
      ? null
      : GrpcInput.ParseGuid(request.CourseId, nameof(request.CourseId));

    return new GetStudentEnrollmentsQuery(studentId, courseId);
  }
}
