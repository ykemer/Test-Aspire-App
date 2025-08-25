using ClassesGRPC;

namespace Service.Courses.Features.Classes.GetClass;

public static class GetClassMapper
{
  public static GetClassQuery ToGetClassQuery(this GrpcGetClassRequest request) => new(request.Id, request.CourseId, [.. request.EnrolledClasses], request.ShowAll);
}
