using ClassesGRPCClient;

using Contracts.Classes.Responses;
using Contracts.Common;
using Contracts.Courses.Requests;

namespace Platform.Features.Classes.ListClasses;

public static class ListClassesMapper
{
  public static GrpcListClassRequest ToGrpcListClassRequest(this ListCoursesRequest request, Guid courseId,
    IReadOnlyList<string> enrolledClassIds, bool showAll) =>
    new()
    {
      CourseId = courseId.ToString(),
      Page = request.PageNumber,
      PageSize = request.PageSize,
      EnrolledClasses = { enrolledClassIds },
      ShowAll = showAll
    };

  public static PagedList<ClassListItemResponse> ToClassListItemResponse(this GrpcListClassResponse response,
    IReadOnlyList<string> enrolledClassIds) =>
    new()
    {
      Items = response.Items.Select(courseClass => new ClassListItemResponse
      {
        Id = Guid.Parse(courseClass.Id),
        CourseId = Guid.Parse(courseClass.CourseId),
        CourseStartDate = courseClass.CourseStartDate.ToDateTime(),
        CourseEndDate = courseClass.CourseEndDate.ToDateTime(),
        RegistrationDeadline = courseClass.RegistrationDeadline.ToDateTime(),
        EnrollmentsCount = courseClass.TotalStudents,
        MaxStudents = courseClass.MaxStudents,
        IsUserEnrolled = enrolledClassIds.Contains(courseClass.Id)
      }).ToList(),
      CurrentPage = response.CurrentPage,
      TotalPages = response.TotalPages,
      PageSize = response.PageSize,
      TotalCount = response.TotalCount
    };
}
