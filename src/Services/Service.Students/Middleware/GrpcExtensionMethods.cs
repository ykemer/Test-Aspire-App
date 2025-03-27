using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;

using StudentsGRPC;

namespace Service.Students.Middleware;

public static class GrpcExtensionMethods
{
  public static GetStudentQuery MapToGetStudentQuery(this GrpcGetStudentByIdRequest request) => new(request.Id);

  public static ListStudentsQuery MapToListStudentsQuery(this GrpcListStudentsRequest request) => new()
  {
    PageNumber = request.Page, PageSize = request.PageSize
  };

  public static DeleteStudentCommand MapToDeleteStudentCommand(this GrpcDeleteStudentRequest request) => new(request.Id);
}
