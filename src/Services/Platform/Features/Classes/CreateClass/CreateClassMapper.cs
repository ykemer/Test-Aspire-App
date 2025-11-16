using ClassesGRPCClient;

using Contracts.Courses.Requests.Classes;
using Contracts.Courses.Responses;

using Google.Protobuf.WellKnownTypes;

namespace Platform.Features.Classes.CreateClass;

public static class CreateClassMapper
{
  public static GrpcCreateClassRequest MapToGrpcCreateClassRequest(this CreateClassRequest request, string courseId) =>
    new()
    {
      CourseId = courseId,
      RegistrationDeadline = request.RegistrationDeadline.ToTimestamp(),
      CourseStartDate = request.CourseStartDate.ToTimestamp(),
      CourseEndDate = request.CourseEndDate.ToTimestamp(),
      MaxStudents = request.MaxStudents
    };


  public static ClassResponse MapToClassResponse(this GrpcClassResponse response) =>
    new()
    {
      Id = response.Id,
      CourseId = response.CourseId,
      RegistrationDeadline = response.RegistrationDeadline.ToDateTime(),
      CourseStartDate = response.CourseStartDate.ToDateTime(),
      CourseEndDate = response.CourseEndDate.ToDateTime(),
      MaxStudents = response.MaxStudents,
      TotalStudents = response.TotalStudents
    };
}
