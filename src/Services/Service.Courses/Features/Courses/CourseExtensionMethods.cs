using ClassesGRPC;

using Contracts.Common;

using CoursesGRPC;

using Google.Protobuf.WellKnownTypes;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses;

public static class CourseExtensionMethods
{
  public static GrpcCourseResponse MapToGrpcCourseResponse(this Course course) =>
    new()
    {
      Id = course.Id, Name = course.Name, Description = course.Description, TotalStudents = course.TotalStudents
    };

  public static GrpcListCoursesResponse MapToGrpcListCoursesResponse(this PagedList<Course> coursesResponse) =>
    new()
    {
      CurrentPage = coursesResponse.CurrentPage,
      PageSize = coursesResponse.PageSize,
      TotalPages = coursesResponse.TotalPages,
      TotalCount = coursesResponse.TotalCount,
      Items = { coursesResponse.Items.Select(i => i.MapToGrpcCourseResponse()) }
    };


  public static GrpcClassResponse MapToGrpcClassResponse(this Class courseClass) =>
    new()
    {
      Id = courseClass.Id,
      CourseId = courseClass.CourseId,
      RegistrationDeadline = DateTime.SpecifyKind(courseClass.RegistrationDeadline, DateTimeKind.Utc).ToTimestamp(),
      CourseStartDate = DateTime.SpecifyKind(courseClass.CourseStartDate, DateTimeKind.Utc).ToTimestamp(),
      CourseEndDate = DateTime.SpecifyKind(courseClass.CourseEndDate, DateTimeKind.Utc).ToTimestamp(),
      MaxStudents = courseClass.MaxStudents,
      TotalStudents = courseClass.TotalStudents
    };


  public static GrpcListClassResponse MapToGrpcListClassResponse(this PagedList<Class> classesResponse) =>
    new()
    {
      CurrentPage = classesResponse.CurrentPage,
      PageSize = classesResponse.PageSize,
      TotalPages = classesResponse.TotalPages,
      TotalCount = classesResponse.TotalCount,
      Items = { classesResponse.Items.Select(i => i.MapToGrpcClassResponse()) }
    };
}
