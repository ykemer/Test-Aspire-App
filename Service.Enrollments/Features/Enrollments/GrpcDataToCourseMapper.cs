using EnrollmentsGRPC;
using Google.Protobuf.WellKnownTypes;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments;

public static class GrpcDataToEnrollmentMapper
{
    public static GrpcEnrollmentResponse EnrollmentToGrpcEnrollmentResponseMap(Enrollment enrollment)
    {
        return new GrpcEnrollmentResponse
        {
            Id = enrollment.Id,
            CourseId = enrollment.CourseId,
            StudentId = enrollment.StudentId,
            StudentLastName = enrollment.StudentFirstName,
            StudentFirstName = enrollment.StudentLastName,
            EnrollmentDateTime = enrollment.EnrollmentDateTime.ToTimestamp()
        };
    }
}