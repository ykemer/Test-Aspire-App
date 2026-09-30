using Contracts.Enrollments.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Platform.Common.Grpc;

namespace Platform.Features.Enrollments.GetCourseEnrollments;

/// <summary>
/// Administrators: lists the students enrolled in any class of one course. Returns 404 if the course does not exist.
/// </summary>
public class GetCourseEnrollmentsEndpoint : EndpointWithoutRequest<ErrorOr<List<EnrollmentResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesClient;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsClient;
  private readonly IGrpcCaller _grpc;

  public GetCourseEnrollmentsEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesClient,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsClient, IGrpcCaller grpc)
  {
    _coursesClient = coursesClient;
    _enrollmentsClient = enrollmentsClient;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId:guid}/enrollments");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Enrollments"));
  }

  public override async Task<ErrorOr<List<EnrollmentResponse>>> ExecuteAsync(CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId").ToString();

    var course = await _grpc.CallAsync(_coursesClient.GetCourseAsync(
      new GrpcGetCourseRequest { Id = courseId, ShowAll = true }, cancellationToken: ct));
    if (course.IsError)
    {
      return course.Errors;
    }

    var enrollments = await _grpc.CallAsync(_enrollmentsClient.GetCourseEnrollmentsAsync(
      new GrpcGetCourseEnrollmentsRequest { CourseId = courseId }, cancellationToken: ct));
    return enrollments.Then(response => response.ToEnrollmentResponseList());
  }
}
