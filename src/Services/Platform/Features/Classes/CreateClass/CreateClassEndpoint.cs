using ClassesGRPCClient;

using Contracts.Courses.Events;
using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Classes;
using Contracts.Courses.Requests.Courses;
using Contracts.Courses.Responses;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;

namespace Platform.Features.Classes.CreateClass;

public class CreateClassEndpoint : Endpoint<CreateClassRequest,
  ErrorOr<ClassResponse>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;
  private readonly IPublishEndpoint _publishEndpoint;

  public CreateClassEndpoint(GrpcClassService.GrpcClassServiceClient classGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache, IPublishEndpoint publishEndpoint)
  {
    _classGrpcService = classGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
    _publishEndpoint = publishEndpoint;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<ClassResponse>> ExecuteAsync(CreateClassRequest createClassCommand,
    CancellationToken ct)
  {

    var id = Route<Guid>("CourseId");
    var request =
      _classGrpcService.CreateClassAsync(createClassCommand.MapToGrpcCreateClassRequest(id.ToString()),
        cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);

    if (result.IsError)
    {
      return result.FirstError;
    }


    var classResponse = result.Value;


    await _publishEndpoint.Publish(
      new ClassCreatedEvent
      {
        CourseId = id.ToString(),
        CourseStartDate = createClassCommand.CourseStartDate,
        CourseEndDate = createClassCommand.CourseEndDate,
        RegistrationDeadline = createClassCommand.RegistrationDeadline,
        MaxStudents = createClassCommand.MaxStudents,
        Id = classResponse.Id
      }, ct);

    await _outputCache.EvictByTagAsync("classes", ct);
    return classResponse.MapToClassResponse();
  }
}
