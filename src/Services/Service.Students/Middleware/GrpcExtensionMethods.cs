using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;
using StudentsGRPC;

namespace Service.Students.Middleware;

public static class GrpcExtensionMethods
{
    public static GetStudentQuery ToGetStudentQuery(this GrpcGetStudentByIdRequest request) =>
        new GetStudentQuery(request.Id);
    
    public static ListStudentsQuery ToListStudentsQuery(this GrpcListStudentsRequest request) => new ListStudentsQuery
    {
        PageNumber = request.Page,
        PageSize = request.PageSize
    };
    
    public static DeleteStudentCommand ToDeleteStudentCommand(this GrpcDeleteStudentRequest request) =>
        new DeleteStudentCommand(request.Id);
}