using ClassesGRPCClient;

using Contracts.Common;
using Contracts.Courses.Requests.Courses;
using Contracts.Courses.Responses;

using Google.Protobuf.Collections;

namespace Platform.Features.Classes.ListClasses;

public static class ListClassMapper
{

  public static GrpcListClassRequest ToGrpcListClassRequest(this ListCoursesRequest request,
    List<string> enrolledClasses, bool showAll, string courseId)
  {
    var repeatedEnrolledClasses = new RepeatedField<string>();
    repeatedEnrolledClasses.AddRange(enrolledClasses);

    return new GrpcListClassRequest
    {
      Page = request.PageNumber,
      PageSize = request.PageSize,
      EnrolledClasses = { repeatedEnrolledClasses },
      ShowAll = showAll,
      CourseId =courseId
    };
  }

  public static PagedList<ClassListItemResponse> ToClassListItemResponse(this GrpcListClassResponse course,
    List<string>? enrollments = null) =>
    new()
    {
      Items = course.Items.Select(i => new ClassListItemResponse
      {
        Id = Guid.Parse(i.Id),
        CourseId = Guid.Parse(i.CourseId),
        CourseStartDate = i.CourseStartDate.ToDateTime(),
        CourseEndDate = i.CourseEndDate.ToDateTime(),
        RegistrationDeadline = i.RegistrationDeadline.ToDateTime(),
        EnrollmentsCount = i.TotalStudents,
        MaxStudents = i.MaxStudents,
        IsUserEnrolled = enrollments.Contains(i.Id)
      }).ToList(),
      CurrentPage = course.CurrentPage,
      TotalPages = course.TotalPages,
      PageSize = course.PageSize,
      TotalCount = course.TotalCount,
    };
}
