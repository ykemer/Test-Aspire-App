using Google.Protobuf.WellKnownTypes;
using Service.Students.Entitites;
using StudentsGRPC;

namespace Service.Students.Features;

public static class GrpcToStudentMapper
{
    public static GrpcStudentResponse StudentToGrpcStudentResponse(Student student)
    {
        return new GrpcStudentResponse
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Birthday = DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc).ToTimestamp(),
            Email = student.Email
        };
    }
}