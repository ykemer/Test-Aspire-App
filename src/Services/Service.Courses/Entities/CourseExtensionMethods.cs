using Contracts.Common;

using CoursesGRPC;

namespace Service.Courses.Entities;

public static class CourseExtensionMethods
{
  public static GrpcCourseResponse MapToGrpcCourseResponse(this Course course) =>
    new()
    {
      Id = course.Id, Name = course.Name, Description = course.Description, TotalStudents = course.EnrollmentsCount
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
}
