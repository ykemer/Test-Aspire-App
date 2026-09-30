using StudentsGRPC;

namespace Service.Students.Features.ListStudents;

public static class ListStudentsMapper
{
  public static ListStudentsQuery ToListStudentsQuery(this GrpcListStudentsRequest request) =>
    new() { PageNumber = request.Page, PageSize = request.PageSize };
}
