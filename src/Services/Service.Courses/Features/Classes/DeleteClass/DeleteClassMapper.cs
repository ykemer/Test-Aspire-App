using ClassesGRPC;

namespace Service.Courses.Features.Classes.DeleteClass;

public static class DeleteClassMapper
{
  public static DeleteClassCommand MapToDeleteClassCommand(this GrpcDeleteClassRequest request) =>
    new(request.Id, request.CourseId);
}
