using ClassesGRPC;

namespace Service.Courses.Features.Classes.ListClasses;

public static class ListClassesMapper
{
  public static ListClassesQuery MapToListClassesRequest(this GrpcListClassRequest request) =>
    new()
    {
      CourseId = request.CourseId,
      PageSize = request.PageSize,
      PageNumber = request.Page,
      EnrolledClasses = [.. request.EnrolledClasses],
      ShowAll = request.ShowAll
    };
}
