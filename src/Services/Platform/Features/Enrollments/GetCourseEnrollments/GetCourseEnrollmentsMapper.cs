using Contracts.Enrollments.Responses;

using EnrollmentsGRPCClient;

namespace Platform.Features.Enrollments.GetCourseEnrollments;

public static class GetCourseEnrollmentsMapper
{
  public static List<EnrollmentResponse> ToEnrollmentResponseList(this GrpcListEnrollmentsResponse response) =>
    response.Items.Select(i => new EnrollmentResponse
    {
      Id = i.Id,
      CourseId = i.CourseId,
      StudentId = i.StudentId,
      EnrollmentDateTime = i.EnrollmentDateTime.ToDateTime(),
      FirstName = i.StudentFirstName,
      LastName = i.StudentLastName
    }).ToList();
}
