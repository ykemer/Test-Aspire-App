using ClassesGRPCClient;

using Contracts.Courses.Events;
using Contracts.Courses.Requests;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;

namespace Platform.Features.Classes.UpdateClass;

public class UpdateClassEndpoint : Endpoint<UpdateClassRequest,
  ErrorOr<Updated>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;
  private readonly IPublishEndpoint _publishEndpoint;

  public UpdateClassEndpoint(GrpcClassService.GrpcClassServiceClient classGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache, IPublishEndpoint publishEndpoint)
  {
    _classGrpcService = classGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
    _publishEndpoint = publishEndpoint;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateClassRequest updateClassCommand,
    CancellationToken ct)
  {

    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var request =
      _classGrpcService.UpdateClassAsync(updateClassCommand.MapToGrpcUpdateClassRequest(courseId.ToString(), classId.ToString()), cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);

    if (result.IsError)
    {
      return result.FirstError;
    }

    await _publishEndpoint.Publish(
      new ClassUpdatedEvent
      {
        CourseId = courseId.ToString(),
        CourseStartDate = updateClassCommand.CourseStartDate,
        CourseEndDate = updateClassCommand.CourseEndDate,
        RegistrationDeadline = updateClassCommand.RegistrationDeadline,
        MaxStudents = updateClassCommand.MaxStudents,
        Id = classId.ToString()
      }, ct);

    await _outputCache.EvictByTagAsync("classes", ct);
    return Result.Updated;
  }
}
