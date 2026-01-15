using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using MassTransit;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Services.User;

namespace Platform.Features.Classes.CreateClass;

public class CreateClassEndpoint : Endpoint<CreateClassRequest,
  ErrorOr<Success>>
{
  private readonly IOutputCacheStore _outputCache;
  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly IUserService _userService;

  public CreateClassEndpoint(IOutputCacheStore outputCache,
    ISendEndpointProvider sendEndpointProvider, IUserService userService)
  {
    _outputCache = outputCache;
    _sendEndpointProvider = sendEndpointProvider;
    _userService = userService;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes");
    Policies("RequireAdministratorRole");
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Success>> ExecuteAsync(CreateClassRequest createClassCommand,
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");
    var userId = _userService.GetUserId(User).ToString();

    var sendUri = new Uri("queue:create-class-command");
    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);

    await endpoint.Send(
      new CreateClassCommand
      {
        CourseId = id.ToString(),
        CourseStartDate = createClassCommand.CourseStartDate,
        CourseEndDate = createClassCommand.CourseEndDate,
        RegistrationDeadline = createClassCommand.RegistrationDeadline,
        MaxStudents = createClassCommand.MaxStudents,
        UserId = userId
      },
      ct);


    await _outputCache.EvictByTagAsync("classes", ct);
    return Result.Success;
  }
}
