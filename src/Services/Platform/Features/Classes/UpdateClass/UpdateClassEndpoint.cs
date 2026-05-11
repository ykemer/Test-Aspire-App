using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Classes.UpdateClass;

public class UpdateClassEndpoint : Endpoint<UpdateClassRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public UpdateClassEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
    _userService = userService;
  }

  public override void Configure()
  {
    Put("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateClassRequest updateClassCommand, CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");
    var userId = _userService.GetUserId(User).ToString();
    await _outputCache.EvictByTagAsync("classes", ct);

    await _bus.Send(new UpdateClassCommand
    {
      ClassId = classId,
      CourseId = courseId,
      RegistrationDeadline = updateClassCommand.RegistrationDeadline,
      CourseStartDate = updateClassCommand.CourseStartDate,
      CourseEndDate = updateClassCommand.CourseEndDate,
      MaxStudents = updateClassCommand.MaxStudents,
      UserId = userId
    });

    return Result.Updated;
  }
}
