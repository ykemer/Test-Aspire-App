using ClassesGRPC;

namespace Service.Courses.Features.Classes.GetClass;

public static class GetClassMapper
{
  public static GetClassQuery ToGetClassQuery(this GrpcGetClassRequest request)
  {
    return new GetClassQuery(
      Guid.Parse(request.Id),
      Guid.Parse(request.CourseId),
      request.EnrolledClasses.Select(Guid.Parse).ToList(),
      request.ShowAll
    );
  }
}
