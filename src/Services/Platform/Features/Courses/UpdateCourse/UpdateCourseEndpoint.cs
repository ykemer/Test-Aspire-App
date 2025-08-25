using Contracts.Courses.Requests;

using CoursesGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;


namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseEndpoint : Endpoint<UpdateCourseRequest,
  ErrorOr<Updated>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;

  public UpdateCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
  }

  public override void Configure()
  {
    Patch("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateCourseRequest updateCourseCommand,
    CancellationToken ct)
  {

    var id = Route<Guid>("CourseId");
    var request =
      _coursesGrpcService.UpdateCourseAsync(updateCourseCommand.ToGrpcUpdateCourseRequest(id.ToString()), cancellationToken: ct);

    var output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    await _outputCache.EvictByTagAsync("courses", ct);
    return output.Match<ErrorOr<Updated>>(
      _ => Result.Updated,
      error => error
    );
  }
}
