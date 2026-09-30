using Contracts.Classes.Commands;
using Contracts.Classes.Requests;

using FastEndpoints;

using Platform.Common.Auth;

using Rebus.Bus;

namespace Platform.Features.Classes.UpdateClass;

/// <summary>
/// Asks the Courses service to change a class. The answer arrives later as a live notification.
/// </summary>
public class UpdateClassEndpoint : Endpoint<UpdateClassRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;

  public UpdateClassEndpoint(IBus bus) => _bus = bus;

  public override void Configure()
  {
    Put("/api/courses/{CourseId:guid}/classes/{ClassId:guid}");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateClassRequest request, CancellationToken ct)
  {
    await _bus.Send(new UpdateClassCommand
    {
      CourseId = Route<Guid>("CourseId"),
      ClassId = Route<Guid>("ClassId"),
      RegistrationDeadline = request.RegistrationDeadline,
      CourseStartDate = request.CourseStartDate,
      CourseEndDate = request.CourseEndDate,
      MaxStudents = request.MaxStudents,
      UserId = User.GetUserId().ToString()
    });

    return Result.Updated;
  }
}
