using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

namespace Platform.Features.Courses.ListCourses;

public static class ListCoursesMapper
{
  public static GrpcListCoursesRequest ToGrpcListCoursesRequest(this ListCoursesRequest request,
    IReadOnlyList<string> enrolledClassIds, bool showAll) =>
    new()
    {
      Page = request.PageNumber,
      PageSize = request.PageSize,
      Query = request.Query ?? string.Empty,
      EnrolledClasses = { enrolledClassIds },
      ShowAll = showAll
    };

  public static PagedList<CourseListItemResponse> ToCourseListItemResponse(this GrpcListCoursesResponse response,
    IReadOnlyList<string> enrolledCourseIds) =>
    new()
    {
      Items = response.Items.Select(course => new CourseListItemResponse
      {
        Id = Guid.Parse(course.Id),
        Name = course.Name,
        Description = course.Description,
        TotalStudents = course.TotalStudents,
        IsUserEnrolled = enrolledCourseIds.Contains(course.Id)
      }).ToList(),
      CurrentPage = response.CurrentPage,
      TotalPages = response.TotalPages,
      PageSize = response.PageSize,
      TotalCount = response.TotalCount
    };
}
