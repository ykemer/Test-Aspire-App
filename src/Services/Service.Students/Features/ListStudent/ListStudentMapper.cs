using StudentsGRPC;

namespace Service.Students.Features.ListStudent;

public static class ListStudentMapper
{
  public static ListStudentsQuery MapToListStudentsQuery(this GrpcListStudentsRequest request) => new()
  {
    PageNumber = request.Page, PageSize = request.PageSize
  };
}
