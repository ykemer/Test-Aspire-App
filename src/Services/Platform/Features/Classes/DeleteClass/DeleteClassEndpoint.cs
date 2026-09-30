using Contracts.Classes.Commands;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Classes.DeleteClass;

/// <summary>
/// Asks the Courses service to delete a class. The answer arrives later as a live notification.
/// </summary>
public class DeleteClassEndpoint : EndpointWithoutRequest<ErrorOr<Deleted>>
{
  private readonly IBus _bus;

  public DeleteClassEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Delete("/api/courses/{CourseId:guid}/classes/{ClassId:guid}");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(CancellationToken ct)
  {
    await _bus.Send(new DeleteClassCommand
    {
      CourseId = Route<Guid>("CourseId"), ClassId = Route<Guid>("ClassId"), UserId = User.GetUserId().ToString()
    });

    return Result.Deleted;
  }
}
