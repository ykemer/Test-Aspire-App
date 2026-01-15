using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Classes.UpdateClass;

public class UpdateClassEndpoint : Endpoint<UpdateClassRequest,
  ErrorOr<Updated>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;


  public UpdateClassEndpoint(IOutputCacheStore outputCache, ISendEndpointProvider sendEndpointProvider,
    IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
  }

  public override void Configure()
  {
    Put("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateClassRequest updateClassCommand,
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("classes", ct);

    var sendUri = new Uri("queue:update-class-command");
    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);

    await endpoint.Send(
      new UpdateClassCommand
      {
        ClassId = classId.ToString(),
        CourseId = courseId.ToString(),
        RegistrationDeadline = updateClassCommand.RegistrationDeadline,
        CourseStartDate = updateClassCommand.CourseStartDate,
        CourseEndDate = updateClassCommand.CourseEndDate,
        MaxStudents = updateClassCommand.MaxStudents,
        UserId = userId
      }, ct);

    return Result.Updated;
  }
}
