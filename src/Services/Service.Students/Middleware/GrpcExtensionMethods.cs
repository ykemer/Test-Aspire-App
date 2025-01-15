using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;

using StudentsGRPC;

namespace Service.Students.Middleware;

public static class GrpcExtensionMethods
{
  public static GetStudentQuery ToGetStudentQuery(this GrpcGetStudentByIdRequest request) => new(request.Id);

  public static ListStudentsQuery ToListStudentsQuery(this GrpcListStudentsRequest request) => new()
  {
    PageNumber = request.Page, PageSize = request.PageSize
  };

  public static DeleteStudentCommand ToDeleteStudentCommand(this GrpcDeleteStudentRequest request) => new(request.Id);
}
