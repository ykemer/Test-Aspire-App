using Contracts.Common;
using Contracts.Courses.Requests.Courses;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using Google.Protobuf.Collections;

namespace Platform.Features.Courses.ListCourses;

public static class ListCoursesMapper
{
  public static GrpcListCoursesRequest ToGrpcGetEnrollmentsByCoursesRequest(this ListCoursesRequest request,
    List<string> enrolledClasses, bool ShowAll)
  {
    var repeatedEnrolledClasses = new RepeatedField<string>();
    repeatedEnrolledClasses.AddRange(enrolledClasses);

    return new GrpcListCoursesRequest
    {
      Page = request.PageNumber,
      PageSize = request.PageSize,
      Query = request.Query ?? "",
      EnrolledClasses = { repeatedEnrolledClasses },
      ShowAll = ShowAll
    };
  }

  public static PagedList<CourseListItemResponse> ToCourseListItemResponse(this GrpcListCoursesResponse course,
    List<string>? enrollments = null) =>
    new()
    {
      Items = course.Items.Select(i => new CourseListItemResponse
      {
        Id = Guid.Parse(i.Id),
        Name = i.Name,
        Description = i.Description,
        TotalStudents = i.TotalStudents,
        IsUserEnrolled = enrollments.Contains(i.Id)
      }).ToList(),
      CurrentPage = course.CurrentPage,
      TotalPages = course.TotalPages,
      PageSize = course.PageSize,
      TotalCount = course.TotalCount
    };
}
