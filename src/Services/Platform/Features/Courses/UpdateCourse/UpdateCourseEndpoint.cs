using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Courses.UpdateCourse;

/// <summary>
/// Asks the Courses service to change a course. The answer arrives later as a live notification.
/// </summary>
public class UpdateCourseEndpoint : Endpoint<UpdateCourseRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;

  public UpdateCourseEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Put("/api/courses/{CourseId:guid}");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateCourseRequest request, CancellationToken ct)
  {
    await _bus.Send(new UpdateCourseCommand
    {
      CourseId = Route<Guid>("CourseId"),
      Name = request.Name,
      Description = request.Description,
      UserId = User.GetUserId().ToString()
    });

    return Result.Updated;
  }
}
