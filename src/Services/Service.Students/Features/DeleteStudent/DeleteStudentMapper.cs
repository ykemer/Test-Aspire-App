using Library.GRPC;

using StudentsGRPC;

namespace Service.Students.Features.DeleteStudent;

public static class DeleteStudentMapper
{
  public static DeleteStudentCommand ToDeleteStudentCommand(this GrpcDeleteStudentRequest request) =>
    new(GrpcInput.ParseGuid(request.Id, nameof(request.Id)));
}
