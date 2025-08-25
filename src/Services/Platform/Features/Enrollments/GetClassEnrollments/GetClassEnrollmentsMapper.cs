using Contracts.Enrollments.Responses;

using EnrollmentsGRPCClient;

namespace Platform.Features.Enrollments.GetClassEnrollments;

public static class GetClassesEnrollmentsMapper
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
