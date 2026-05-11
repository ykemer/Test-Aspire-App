using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseEndpoint : Endpoint<UpdateCourseRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public UpdateCourseEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
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

    await _bus.Send(new UpdateCourseCommand
    {
      CourseId = id,
      Name = updateCourseCommand.Name,
      Description = updateCourseCommand.Description,
      UserId = userId
    });

    return Result.Updated;
  }
}
