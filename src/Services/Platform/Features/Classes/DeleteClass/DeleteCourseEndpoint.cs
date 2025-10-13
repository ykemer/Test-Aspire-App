using ClassesGRPCClient;

using Contracts.Courses.Events;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;

namespace Platform.Features.Classes.DeleteClass;

public class DeleteClassEndpoint : EndpointWithoutRequest<
  ErrorOr<Deleted>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteClassEndpoint(GrpcClassService.GrpcClassServiceClient classGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache, IPublishEndpoint publishEndpoint)
  {
    _classGrpcService = classGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
    _publishEndpoint = publishEndpoint;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var request =
      _classGrpcService.DeleteClassAsync(new GrpcDeleteClassRequest
      {
        Id = classId.ToString(),
        CourseId = courseId.ToString()
      }, cancellationToken: ct);

    var output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    await _publishEndpoint.Publish(
      new ClassDeletedEvent() { CourseId = courseId.ToString(), ClassId = classId.ToString()}, ct);
    await _outputCache.EvictByTagAsync("courses", ct);
    return output.Match<ErrorOr<Deleted>>(
      _ => Result.Deleted,
      error => error
    );
  }
}
