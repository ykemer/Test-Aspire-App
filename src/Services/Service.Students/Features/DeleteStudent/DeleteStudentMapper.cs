using StudentsGRPC;

namespace Service.Students.Features.DeleteStudent;

public static class DeleteStudentMapper
{
  public static DeleteStudentCommand MapToDeleteStudentCommand(this GrpcDeleteStudentRequest request) =>
    new(Guid.Parse(request.Id));
}
