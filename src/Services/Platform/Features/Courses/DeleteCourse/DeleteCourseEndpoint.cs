using Contracts.Courses.Events;

using CoursesGRPCClient;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseEndpoint : EndpointWithoutRequest<
  ErrorOr<Deleted>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache, IPublishEndpoint publishEndpoint)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
    _publishEndpoint = publishEndpoint;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");

    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var request =
      _coursesGrpcService.DeleteCourseAsync(new GrpcDeleteCourseRequest { Id = id.ToString() }, cancellationToken: ct);

    var output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    await _outputCache.EvictByTagAsync("courses", ct);

    await _publishEndpoint.Publish(
      new CourseDeletedEvent { CourseId = id.ToString() }, ct);


    return output.Match<ErrorOr<Deleted>>(
      _ => Result.Deleted,
      error => error
    );
  }
}
