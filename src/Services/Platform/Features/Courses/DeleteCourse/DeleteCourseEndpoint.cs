using Contracts.Courses.Commands;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseEndpoint : EndpointWithoutRequest<ErrorOr<Deleted>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public DeleteCourseEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
    _userService = userService;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("courses", ct);

    await _bus.Send(new DeleteCourseCommand { CourseId = id, UserId = userId });

    return Result.Deleted;
  }
}
