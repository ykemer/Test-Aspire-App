using Contracts.Students.Responses;

using StudentsGRPCClient;

namespace Platform.Features.Students.GetStudent;

public static class GetStudentMapper
{
  public static StudentResponse ToStudentResponse(this GrpcStudentResponse student) =>
    new()
    {
      Id = Guid.Parse(student.Id),
      FirstName = student.FirstName,
      LastName = student.LastName,
      Email = student.Email,
      DateOfBirth = student.Birthday.ToDateTime(),
      EnrollmentCount = student.EnrolledCourses
    };
}
