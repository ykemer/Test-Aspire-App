using Contracts.Enrollments.Responses;

using EnrollmentsGRPCClient;

namespace Platform.Features.Enrollments;

public static class EnrollmentResponseMapper
{
  public static List<EnrollmentResponse> ToEnrollmentResponseList(this GrpcListEnrollmentsResponse response) =>
    response.Items.Select(enrollment => new EnrollmentResponse
    {
      Id = enrollment.Id,
      CourseId = Guid.Parse(enrollment.CourseId),
      StudentId = Guid.Parse(enrollment.StudentId),
      EnrollmentDateTime = enrollment.EnrollmentDateTime.ToDateTime(),
      FirstName = enrollment.StudentFirstName,
      LastName = enrollment.StudentLastName
    }).ToList();
}
