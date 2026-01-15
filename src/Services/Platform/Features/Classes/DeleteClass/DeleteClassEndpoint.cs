using Contracts.Classes.Commands;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Classes.DeleteClass;

public class DeleteClassEndpoint : EndpointWithoutRequest<
  ErrorOr<Deleted>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;

  public DeleteClassEndpoint(IOutputCacheStore outputCache, ISendEndpointProvider sendEndpointProvider,
    IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var userId = _userService.GetUserId(User).ToString();

    var sendUri = new Uri("queue:delete-class-command");
    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);


    await _outputCache.EvictByTagAsync("classes", ct);
    await endpoint.Send(new DeleteClassCommand
    {
      ClassId = classId.ToString(), CourseId = courseId.ToString(), UserId = userId
    });

    return Result.Deleted;
  }
}
