using Contracts.Courses.Commands;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseEndpoint : EndpointWithoutRequest<
  ErrorOr<Deleted>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;

  public DeleteCourseEndpoint(IOutputCacheStore outputCache, ISendEndpointProvider sendEndpointProvider,
    IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
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
    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("courses", ct);
    var sendUri = new Uri("queue:delete-course-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);
    await endpoint.Send(new DeleteCourseCommand { CourseId = id.ToString(), UserId = userId }, ct);

    return Result.Deleted;
  }
}
