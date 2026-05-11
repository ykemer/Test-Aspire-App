using Contracts.Classes.Commands;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Classes.DeleteClass;

public class DeleteClassEndpoint : EndpointWithoutRequest<ErrorOr<Deleted>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public DeleteClassEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
    _userService = userService;
  }

  public override void Configure()
  {
    Delete("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var userId = _userService.GetUserId(User).ToString();

    await _outputCache.EvictByTagAsync("classes", ct);
    await _bus.Send(new DeleteClassCommand { ClassId = classId, CourseId = courseId, UserId = userId });

    return Result.Deleted;
  }
}
