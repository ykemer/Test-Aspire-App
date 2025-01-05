using Contracts.Common;
using Google.Protobuf.WellKnownTypes;
using StudentsGRPC;

namespace Service.Students.Entities;

public static class StudentExtensionMethods
{
    public static GrpcStudentResponse ToGrpcStudentResponse(this Student student)
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
    
    public static GrpcListStudentsResponse ToGrpcListStudentsResponse(this PagedList<Student> studentsResult)
    {
        return new GrpcListStudentsResponse
        {
            Items = { studentsResult.Items.Select(i => i.ToGrpcStudentResponse()) },
            TotalCount = studentsResult.TotalCount,
            PageSize = studentsResult.PageSize,
            CurrentPage = studentsResult.CurrentPage,
            TotalPages = studentsResult.TotalPages,
        };
    }
}