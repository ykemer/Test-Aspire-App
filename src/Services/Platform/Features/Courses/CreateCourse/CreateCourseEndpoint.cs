using Contracts.Courses.Commands;
using Contracts.Courses.Requests;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Courses.CreateCourse;

/// <summary>
/// Asks the Courses service to create a course. The answer arrives later as a live notification
/// (see <see cref="CourseCreatedEventConsumer"/> and <see cref="CourseCreateRejectionEventConsumer"/>).
/// </summary>
public class CreateCourseEndpoint : Endpoint<CreateCourseRequest, ErrorOr<Success>>
{
  private readonly IBus _bus;

  public CreateCourseEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Post("/api/courses");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Courses"));
  }

  public override async Task<ErrorOr<Success>> ExecuteAsync(CreateCourseRequest request, CancellationToken ct)
  {
    await _bus.Send(new CreateCourseCommand
    {
      Name = request.Name, Description = request.Description, UserId = User.GetUserId().ToString()
    });

    return Result.Success;
  }
}
