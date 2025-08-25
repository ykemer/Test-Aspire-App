using StudentsGRPC;

namespace Service.Students.Features.GetStudent;

public static class GetStudentMapper
{
  public static GetStudentQuery MapToGetStudentQuery(this GrpcGetStudentByIdRequest request) => new(request.Id);
}
