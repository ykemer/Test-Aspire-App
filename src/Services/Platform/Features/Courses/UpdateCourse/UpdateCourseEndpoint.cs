using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseEndpoint : Endpoint<UpdateCourseRequest,
  ErrorOr<Updated>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;

  public UpdateCourseEndpoint(IOutputCacheStore outputCache,
    ISendEndpointProvider sendEndpointProvider,
    IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
  }

  public override void Configure()
  {
    Put("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateCourseRequest updateCourseCommand,
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("courses", ct);
    var sendUri = new Uri("queue:update-course-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);
    await endpoint.Send(
      new UpdateCourseCommand
      {
        CourseId = id.ToString(),
        Name = updateCourseCommand.Name,
        Description = updateCourseCommand.Description,
        UserId = userId
      }, ct);

    return Result.Updated;
  }
}
