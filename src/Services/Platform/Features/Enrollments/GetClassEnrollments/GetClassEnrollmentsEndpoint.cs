using ClassesGRPCClient;

using Contracts.Enrollments.Responses;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Platform.Common.Middleware.Grpc;

namespace Platform.Features.Enrollments.GetClassEnrollments;

public class GetClassEnrollmentsEndpoint : EndpointWithoutRequest<ErrorOr<List<EnrollmentResponse>>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public GetClassEnrollmentsEndpoint(GrpcClassService.GrpcClassServiceClient classesGrpcService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _classesGrpcService = classesGrpcService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId}/classes/{ClassId}/enrollments");
    Policies("RequireAdministratorRole");
    Options(x => x.RequireRateLimiting("fixed-per-user"));
    Description(x => x.WithTags("Enrollments"));
  }


  public override async Task<ErrorOr<List<EnrollmentResponse>>> ExecuteAsync(
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var coursesRequest =
      _classesGrpcService.GetClassAsync(
        new GrpcGetClassRequest { Id = classId.ToString(), CourseId = courseId.ToString() }, cancellationToken: ct);

    var classResult = await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);
    if (classResult.IsError)
    {
      return classResult.FirstError;
    }

    var enrollmentsRequest =
      _enrollmentsGrpcService.GetClassEnrollmentsAsync(new GrpcGetClassEnrollmentsRequest
      {
        CourseId = courseId.ToString(), ClassId = classId.ToString()
      });

    var enrollmentsResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);

    return enrollmentsResult.Match<ErrorOr<List<EnrollmentResponse>>>(
      data => data.ToEnrollmentResponseList(),
      error => error);
  }
}
