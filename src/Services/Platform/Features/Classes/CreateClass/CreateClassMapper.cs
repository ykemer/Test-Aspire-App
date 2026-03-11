using ClassesGRPCClient;

using Contracts.Classes.Responses;
using Contracts.Courses.Responses;

namespace Platform.Features.Classes.CreateClass;

public static class CreateClassMapper
{
  public static ClassResponse MapToClassResponse(this GrpcClassResponse response) =>
    new()
    {
      Id = Guid.Parse(response.Id),
      CourseId = Guid.Parse(response.CourseId),
      RegistrationDeadline = response.RegistrationDeadline.ToDateTime(),
      CourseStartDate = response.CourseStartDate.ToDateTime(),
      CourseEndDate = response.CourseEndDate.ToDateTime(),
      MaxStudents = response.MaxStudents,
      TotalStudents = response.TotalStudents
    };
}
