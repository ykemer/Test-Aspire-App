using EnrollmentsGRPC;

using Google.Protobuf.WellKnownTypes;

namespace Service.Enrollments.Common.Database.Entities;

public static class EnrollmentExtensionMethods
{
  public static GrpcEnrollmentResponse MapToGrpcEnrollmentResponse(this Enrollment enrollment) =>
    new()
    {
      Id = enrollment.Id,
      CourseId = enrollment.CourseId,
      ClassId = enrollment.ClassId,
      StudentId = enrollment.StudentId,
      StudentLastName = enrollment.StudentLastName,
      StudentFirstName = enrollment.StudentFirstName,
      EnrollmentDateTime = DateTime.SpecifyKind(enrollment.EnrollmentDateTime, DateTimeKind.Utc).ToTimestamp()
    };

  public static GrpcListEnrollmentsResponse MapToGrpcListEnrollmentsResponse(this List<Enrollment> enrollments) =>
    new() { Items = { enrollments.Select(i => i.MapToGrpcEnrollmentResponse()) } };
}
