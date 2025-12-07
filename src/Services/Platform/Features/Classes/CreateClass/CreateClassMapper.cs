using ClassesGRPCClient;

using Contracts.Classes.Requests;
using Contracts.Courses.Responses;

using Google.Protobuf.WellKnownTypes;

namespace Platform.Features.Classes.CreateClass;

public static class CreateClassMapper
{
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
