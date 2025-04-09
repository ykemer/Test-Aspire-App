using Contracts.Courses.Requests;

using CoursesGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseEndpoint : EndpointWithoutRequest<
  ErrorOr<Deleted>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;

  public DeleteCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var request =
      _coursesGrpcService.DeleteCourseAsync(new GrpcDeleteCourseRequest { Id = id.ToString()}, cancellationToken: ct);

    var output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    await _outputCache.EvictByTagAsync("courses", ct);
    return output.Match<ErrorOr<Deleted>>(
      _ => Result.Deleted,
      error => error
    );
  }
}
