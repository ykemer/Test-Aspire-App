using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using MassTransit;

using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Service.Enrollments.Common.AsyncDataServices.Consumers;

public class CreateEnrollmentCommandConsumer: IConsumer<CreateEnrollmentCommand>
{
  private readonly IMediator _mediator;

  public CreateEnrollmentCommandConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<CreateEnrollmentCommand> context)
  {
    var result = await _mediator.Send(context.Message.MapToEnrollStudentToClassCommand());

    if (result.IsError)
    {
      await context.Publish(new EnrollmentCreateRequestRejectedEvent
      {
        ClassId = context.Message.ClassId,
        CourseId = context.Message.CourseId,
        StudentId = context.Message.StudentId,
        Reason = result.FirstError.Description
      });
    }
    else
    {
      await context.Publish(new EnrollmentCreatedEvent
      {
        ClassId = context.Message.ClassId,
        CourseId = context.Message.CourseId,
        StudentId = context.Message.StudentId,
      });
    }
  }
}
