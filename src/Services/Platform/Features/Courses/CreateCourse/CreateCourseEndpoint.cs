using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseEndpoint : Endpoint<CreateCourseRequest, ErrorOr<Success>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public CreateCourseEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
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

    await _bus.Send(new CreateCourseCommand
    {
      Name = createCourseCommand.Name, Description = createCourseCommand.Description, UserId = userId
    });

    return Result.Success;
  }
}
