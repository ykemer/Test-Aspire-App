using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

using Rebus.Bus;

namespace Platform.Features.Classes.CreateClass;

public class CreateClassEndpoint : Endpoint<CreateClassRequest, ErrorOr<Success>>
{
  private readonly IBus _bus;
  private readonly IOutputCacheStore _outputCache;
  private readonly IUserService _userService;

  public CreateClassEndpoint(IOutputCacheStore outputCache, IBus bus, IUserService userService)
  {
    _outputCache = outputCache;
    _bus = bus;
    _userService = userService;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Success>> ExecuteAsync(CreateClassRequest createClassCommand, CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var userId = _userService.GetUserId(User).ToString();

    await _bus.Send(new CreateClassCommand
    {
      CourseId = id,
      CourseStartDate = createClassCommand.CourseStartDate,
      CourseEndDate = createClassCommand.CourseEndDate,
      RegistrationDeadline = createClassCommand.RegistrationDeadline,
      MaxStudents = createClassCommand.MaxStudents,
      UserId = userId
    });

    await _outputCache.EvictByTagAsync("classes", ct);
    return Result.Success;
  }
}
