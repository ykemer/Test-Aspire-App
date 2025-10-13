using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using MassTransit;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class DeleteEnrollmentCommandConsumer : IConsumer<DeleteEnrollmentCommand>
{
  private readonly IMediator _mediator;

  public DeleteEnrollmentCommandConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<DeleteEnrollmentCommand> context)
  {
    var result = await _mediator.Send(context.Message.MapToUnenrollStudentFromClassCommand());

    if (result.IsError)
    {
      await context.Publish(new EnrollmentDeleteRequestRejectedEvent
      {
        ClassId = context.Message.ClassId,
        CourseId = context.Message.CourseId,
        StudentId = context.Message.StudentId,
        Reason = result.FirstError.Description
      });
    }
    else
    {
      await context.Publish(new EnrollmentDeletedEvent
      {
        CourseId = context.Message.CourseId, ClassId = context.Message.ClassId, StudentId = context.Message.StudentId
      });
    }
  }
}
