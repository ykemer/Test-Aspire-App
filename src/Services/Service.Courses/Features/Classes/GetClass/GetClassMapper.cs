using ClassesGRPC;

using Library.GRPC;

namespace Service.Courses.Features.Classes.GetClass;

public static class GetClassMapper
{
  public static GetClassQuery ToGetClassQuery(this GrpcGetClassRequest request) =>
    new(
      GrpcInput.ParseGuid(request.Id, nameof(request.Id)),
      GrpcInput.ParseGuid(request.CourseId, nameof(request.CourseId)),
      GrpcInput.ParseGuidList(request.EnrolledClasses, nameof(request.EnrolledClasses)),
      request.ShowAll
    );
}
