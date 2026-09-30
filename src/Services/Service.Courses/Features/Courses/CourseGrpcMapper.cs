using Contracts.Common;

using CoursesGRPC;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses;

public static class CourseGrpcMapper
{
  public static GrpcCourseResponse MapToGrpcCourseResponse(this Course course) =>
    new()
    {
      Id = course.Id.ToString(),
      Name = course.Name,
      Description = course.Description,
      TotalStudents = course.TotalStudents
    };

  public static GrpcListCoursesResponse MapToGrpcListCoursesResponse(this PagedList<Course> courses) =>
    new()
    {
      CurrentPage = courses.CurrentPage,
      PageSize = courses.PageSize,
      TotalPages = courses.TotalPages,
      TotalCount = courses.TotalCount,
      Items = { courses.Items.Select(course => course.MapToGrpcCourseResponse()) }
    };
}
