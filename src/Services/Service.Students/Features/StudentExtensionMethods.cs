using Contracts.Common;

using Google.Protobuf.WellKnownTypes;

using Service.Students.Database.Entities;

using StudentsGRPC;

namespace Service.Students.Features;

public static class StudentExtensionMethods
{
  public static GrpcStudentResponse MapToGrpcStudentResponse(this Student student) =>
    new()
    {
      Id = student.Id,
      FirstName = student.FirstName,
      LastName = student.LastName,
      Birthday = DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc).ToTimestamp(),
      Email = student.Email
    };

  public static GrpcListStudentsResponse MapToGrpcListStudentsResponse(this PagedList<Student> studentsResult) =>
    new()
    {
      Items = { studentsResult.Items.Select(i => i.MapToGrpcStudentResponse()) },
      TotalCount = studentsResult.TotalCount,
      PageSize = studentsResult.PageSize,
      CurrentPage = studentsResult.CurrentPage,
      TotalPages = studentsResult.TotalPages
    };
}
