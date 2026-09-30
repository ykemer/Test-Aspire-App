using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

/// <summary>
/// Handles an "unenroll" request from the Platform saga and publishes the result (deleted or rejected).
/// </summary>
public class DeleteEnrollmentCommandConsumer : IHandleMessages<DeleteEnrollmentCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public DeleteEnrollmentCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(DeleteEnrollmentCommand message)
  {
    var result = await _mediator.Send(message.ToUnenrollStudentFromClassCommand());

    if (result.IsError)
    {
      await _bus.Publish(new EnrollmentDeleteRequestRejectedEvent
      {
        ClassId = message.ClassId,
        CourseId = message.CourseId,
        StudentId = message.StudentId,
        Reason = result.FirstError.Description
      });
      return;
    }

    await _bus.Publish(new EnrollmentDeletedEvent
    {
      CourseId = message.CourseId, ClassId = message.ClassId, StudentId = message.StudentId
    });
  }
}
