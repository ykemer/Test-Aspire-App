using Library.GRPC;

using StudentsGRPC;

namespace Service.Students.Features.GetStudent;

public static class GetStudentMapper
{
  public static GetStudentQuery ToGetStudentQuery(this GrpcGetStudentByIdRequest request) =>
    new(GrpcInput.ParseGuid(request.Id, nameof(request.Id)));
}
