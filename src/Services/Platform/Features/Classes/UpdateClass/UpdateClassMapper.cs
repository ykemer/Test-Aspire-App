using ClassesGRPCClient;

using Contracts.Courses.Requests;

using Google.Protobuf.WellKnownTypes;

namespace Platform.Features.Classes.UpdateClass;

public static class UpdateClassMapper
{
  public static GrpcUpdateClassRequest MapToGrpcUpdateClassRequest(this UpdateClassRequest request, string courseId,
    string classId) =>
    new()
    {
      CourseId = courseId,
      Id = classId,
      RegistrationDeadline = request.RegistrationDeadline.ToTimestamp(),
      CourseStartDate = request.CourseStartDate.ToTimestamp(),
      CourseEndDate = request.CourseEndDate.ToTimestamp(),
      MaxStudents = request.MaxStudents
    };
}
