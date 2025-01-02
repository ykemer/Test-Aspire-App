using Contracts.Students.Responses;
using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

public static class GrpcStudentToStudentResponseMapper
{
    public static StudentResponse MapToStudentResponse(GrpcStudentResponse student)
    {
        return new StudentResponse
        {
            Id = Guid.Parse(student.Id),
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            DateOfBirth = student.Birthday.ToDateTime(),
            EnrollmentCount = student.EnrolledCourses
        };
    }
}