using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Classes.CreateClass;

/// <summary>
/// Asks the Courses service to add a class to a course. The answer arrives later as a live notification.
/// </summary>
public class CreateClassEndpoint : Endpoint<CreateClassRequest, ErrorOr<Success>>
{
  private readonly IBus _bus;

  public CreateClassEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Post("/api/courses/{CourseId:guid}/classes");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Success>> ExecuteAsync(CreateClassRequest request, CancellationToken ct)
  {
    await _bus.Send(new CreateClassCommand
    {
      CourseId = Route<Guid>("CourseId"),
      RegistrationDeadline = request.RegistrationDeadline,
      CourseStartDate = request.CourseStartDate,
      CourseEndDate = request.CourseEndDate,
      MaxStudents = request.MaxStudents,
      UserId = User.GetUserId().ToString()
    });

    return Result.Success;
  }
}
