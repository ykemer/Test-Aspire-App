using ClassesGRPC;

using Contracts.Common;

using Google.Protobuf.WellKnownTypes;

using Library.Dates;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes;

public static class ClassGrpcMapper
{
  public static GrpcClassResponse MapToGrpcClassResponse(this Class courseClass) =>
    new()
    {
      Id = courseClass.Id.ToString(),
      CourseId = courseClass.CourseId.ToString(),
      RegistrationDeadline = ToUtcTimestamp(courseClass.RegistrationDeadline),
      CourseStartDate = ToUtcTimestamp(courseClass.CourseStartDate),
      CourseEndDate = ToUtcTimestamp(courseClass.CourseEndDate),
      MaxStudents = courseClass.MaxStudents,
      TotalStudents = courseClass.TotalStudents
    };

  public static GrpcListClassResponse MapToGrpcListClassResponse(this PagedList<Class> classes) =>
    new()
    {
      CurrentPage = classes.CurrentPage,
      PageSize = classes.PageSize,
      TotalPages = classes.TotalPages,
      TotalCount = classes.TotalCount,
      Items = { classes.Items.Select(courseClass => courseClass.MapToGrpcClassResponse()) }
    };

  // Protobuf timestamps only accept UTC dates.
  private static Timestamp ToUtcTimestamp(DateTime date) => date.AsUtc().ToTimestamp();
}
