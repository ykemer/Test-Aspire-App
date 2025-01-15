using EnrollmentsGRPC;

using Google.Protobuf.WellKnownTypes;

namespace Service.Enrollments.Entities;

public static class EnrollmentExtensionMethods
{
  public static GrpcEnrollmentResponse ToGrpcEnrollmentResponse(this Enrollment enrollment) =>
    new()
    {
      Id = enrollment.Id,
      CourseId = enrollment.CourseId,
      StudentId = enrollment.StudentId,
      StudentLastName = enrollment.StudentLastName,
      StudentFirstName = enrollment.StudentFirstName,
      EnrollmentDateTime = DateTime.SpecifyKind(enrollment.EnrollmentDateTime, DateTimeKind.Utc).ToTimestamp()
    };

  public static GrpcListEnrollmentsResponse ToGrpcListEnrollmentsResponse(this List<Enrollment> enrollments) =>
    new() { Items = { enrollments.Select(i => i.ToGrpcEnrollmentResponse()) } };
}
