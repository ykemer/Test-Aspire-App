using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseEndpoint : Endpoint<CreateCourseRequest,
  ErrorOr<Success>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;

  public CreateCourseEndpoint(IOutputCacheStore outputCache, ISendEndpointProvider sendEndpointProvider, IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
  }

  public override void Configure()
  {
    Post("/api/courses");
    Policies("RequireAdministratorRole");

    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Success>> ExecuteAsync(CreateCourseRequest createCourseCommand,
    CancellationToken ct)
  {

    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("courses", ct);
    var sendUri = new Uri("queue:create-course-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);
    await endpoint.Send(
      new CreateCourseCommand { Name = createCourseCommand.Name, Description = createCourseCommand.Description, UserId = userId },
      cancellationToken: ct);

    return Result.Success;
  }
}
