using EnrollmentsGRPC;

using Google.Protobuf.WellKnownTypes;

using Library.Dates;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments;

public static class EnrollmentGrpcMapper
{
  public static GrpcEnrollmentResponse MapToGrpcEnrollmentResponse(this Enrollment enrollment) =>
    new()
    {
      Id = enrollment.Id.ToString(),
      CourseId = enrollment.CourseId.ToString(),
      ClassId = enrollment.ClassId.ToString(),
      StudentId = enrollment.StudentId.ToString(),
      StudentLastName = enrollment.StudentLastName,
      StudentFirstName = enrollment.StudentFirstName,
      EnrollmentDateTime = ToUtcTimestamp(enrollment.EnrollmentDateTime)
    };

  public static GrpcListEnrollmentsResponse MapToGrpcListEnrollmentsResponse(this List<Enrollment> enrollments) =>
    new() { Items = { enrollments.Select(enrollment => enrollment.MapToGrpcEnrollmentResponse()) } };

  // Protobuf timestamps only accept UTC dates.
  private static Timestamp ToUtcTimestamp(DateTime date) => date.AsUtc().ToTimestamp();
}
