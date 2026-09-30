using Contracts.Common;

using Google.Protobuf.WellKnownTypes;

using Library.Dates;

using Service.Students.Common.Database.Entities;

using StudentsGRPC;

namespace Service.Students.Features;

public static class StudentGrpcMapper
{
  public static GrpcStudentResponse MapToGrpcStudentResponse(this Student student) =>
    new()
    {
      Id = student.Id.ToString(),
      FirstName = student.FirstName,
      LastName = student.LastName,
      Email = student.Email,
      Birthday = student.DateOfBirth.AsUtc().ToTimestamp(),
      EnrolledCourses = student.EnrollmentsCount
    };

  public static GrpcListStudentsResponse MapToGrpcListStudentsResponse(this PagedList<Student> students) =>
    new()
    {
      Items = { students.Items.Select(student => student.MapToGrpcStudentResponse()) },
      TotalCount = students.TotalCount,
      PageSize = students.PageSize,
      CurrentPage = students.CurrentPage,
      TotalPages = students.TotalPages
    };
}
