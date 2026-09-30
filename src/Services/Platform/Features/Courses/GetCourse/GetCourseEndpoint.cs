using Contracts.Courses.Responses;

using CoursesGRPCClient;

using FastEndpoints;

using Platform.Common.Auth;
using Platform.Common.Grpc;

namespace Platform.Features.Courses.GetCourse;

/// <summary>
/// Returns one course. Students see a course only if it has a class open for registration,
/// or a class they are already enrolled in. Administrators see every course.
/// </summary>
public class GetCourseEndpoint : EndpointWithoutRequest<ErrorOr<CourseResponse>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesClient;
  private readonly ICurrentStudentEnrollments _currentStudentEnrollments;
  private readonly IGrpcCaller _grpc;

  public GetCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesClient,
    ICurrentStudentEnrollments currentStudentEnrollments, IGrpcCaller grpc)
  {
    _coursesClient = coursesClient;
    _currentStudentEnrollments = currentStudentEnrollments;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId:guid}");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(CancellationToken ct)
  {
    var enrollments = await _currentStudentEnrollments.GetAsync(User, ct);
    if (enrollments.IsError)
    {
      return enrollments.Errors;
    }

    var request = new GrpcGetCourseRequest
    {
      Id = Route<Guid>("CourseId").ToString(),
      EnrolledClasses = { enrollments.Value.ClassIds }, // class ids, not course ids: Courses matches classes
      ShowAll = User.IsAdministrator()
    };

    var result = await _grpc.CallAsync(_coursesClient.GetCourseAsync(request, cancellationToken: ct));
    return result.Then(course => course.ToCourseResponse());
  }
}
