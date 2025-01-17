using Contracts.Enrollments.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Enrollments.GetCourseEnrollments;

public class GetCourseEnrollmentsEndpoint : EndpointWithoutRequest<ErrorOr<List<EnrollmentResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public GetCourseEnrollmentsEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _enrollmentsGrpcService = enrollmentsGrpcService;
  }

  public override void Configure()
  {
    Get("/api/courses/enrollments/{CourseId}");
    Policies("RequireAdministratorRole");
  }


  public override async Task<ErrorOr<List<EnrollmentResponse>>> ExecuteAsync(
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var coursesRequest =
      _coursesGrpcService.GetCourseAsync(new GrpcGetCourseRequest { Id = id.ToString() }, cancellationToken: ct);

    var coursesResult = await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);
    if (coursesResult.IsError)
    {
      return coursesResult.FirstError;
    }

    var course = coursesResult.Value;
    var enrollmentsRequest =
      _enrollmentsGrpcService.GetCourseEnrollmentsAsync(new GrpcGetCourseEnrollmentsRequest { CourseId = course.Id });
    var enrollmentsResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);

    return enrollmentsResult.Match<ErrorOr<List<EnrollmentResponse>>>(
      data => data.ToEnrollmentResponseList(),
      error => error);
  }
}
