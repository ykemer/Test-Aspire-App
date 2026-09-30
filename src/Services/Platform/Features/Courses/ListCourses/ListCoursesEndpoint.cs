using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using FastEndpoints;

using Platform.Common.Auth;
using Platform.Common.Grpc;

namespace Platform.Features.Courses.ListCourses;

/// <summary>
/// Returns one page of courses, optionally filtered by search text. Students see courses with an open class
/// or a class they are in, each marked with "is the user enrolled". Administrators see every course.
/// </summary>
public class ListCoursesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<CourseListItemResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesClient;
  private readonly ICurrentStudentEnrollments _currentStudentEnrollments;
  private readonly IGrpcCaller _grpc;

  public ListCoursesEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesClient,
    ICurrentStudentEnrollments currentStudentEnrollments, IGrpcCaller grpc)
  {
    _coursesClient = coursesClient;
    _currentStudentEnrollments = currentStudentEnrollments;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<PagedList<CourseListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var enrollments = await _currentStudentEnrollments.GetAsync(User, ct);
    if (enrollments.IsError)
    {
      return enrollments.Errors;
    }

    var request = query.ToGrpcListCoursesRequest(enrollments.Value.ClassIds, User.IsAdministrator());

    var result = await _grpc.CallAsync(_coursesClient.ListCoursesAsync(request, cancellationToken: ct));
    return result.Then(courses => courses.ToCourseListItemResponse(enrollments.Value.CourseIds));
  }
}
