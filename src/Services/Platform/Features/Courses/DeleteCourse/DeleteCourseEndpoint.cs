using Contracts.Courses.Commands;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Courses.DeleteCourse;

/// <summary>
/// Asks the Courses service to delete a course. The answer arrives later as a live notification.
/// </summary>
public class DeleteCourseEndpoint : EndpointWithoutRequest<ErrorOr<Deleted>>
{
  private readonly IBus _bus;

  public DeleteCourseEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Delete("/api/courses/{CourseId:guid}");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(CancellationToken ct)
  {
    await _bus.Send(new DeleteCourseCommand
    {
      CourseId = Route<Guid>("CourseId"), UserId = User.GetUserId().ToString()
    });

    return Result.Deleted;
  }
}
