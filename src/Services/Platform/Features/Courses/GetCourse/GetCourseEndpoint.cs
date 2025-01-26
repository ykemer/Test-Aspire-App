using Contracts.Courses.Responses;

using CoursesGRPCClient;

using FastEndpoints;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.GetCourse;

public class GetCourseEndpoint : EndpointWithoutRequest<ErrorOr<CourseResponse>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public GetCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");
  }


  public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(
    CancellationToken ct)
  {
    throw new NotImplementedException();
    var id = Route<Guid>("CourseId");
    var request =
      _coursesGrpcService.GetCourseAsync(new GrpcGetCourseRequest { Id = id.ToString() }, cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    return result.Match<ErrorOr<CourseResponse>>(
      data => data.ToCourseResponse(),
      error => error);
  }
}
