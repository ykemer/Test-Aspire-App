using ClassesGRPC;

using Library.GRPC;

namespace Service.Courses.Features.Classes.ListClasses;

public static class ListClassesMapper
{
  public static ListClassesQuery ToListClassesQuery(this GrpcListClassRequest request) =>
    new()
    {
      CourseId = GrpcInput.ParseGuid(request.CourseId, nameof(request.CourseId)),
      PageSize = request.PageSize,
      PageNumber = request.Page,
      EnrolledClasses = GrpcInput.ParseGuidList(request.EnrolledClasses, nameof(request.EnrolledClasses)),
      ShowAll = request.ShowAll
    };
}
