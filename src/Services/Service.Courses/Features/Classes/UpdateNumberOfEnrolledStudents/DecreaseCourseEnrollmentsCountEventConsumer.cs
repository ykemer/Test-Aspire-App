using Contracts.Courses.Events.DecreaseClassEnrollmentsCount;

using MassTransit;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public class DecreaseCourseEnrollmentsCountEventConsumer : IConsumer<DecreaseClassEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;

  public DecreaseCourseEnrollmentsCountEventConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<DecreaseClassEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(context.Message.CourseId, context.Message.ClassId, false));
    if (result.IsError)
    {
      await context.Publish(new DecreaseClassEnrollmentsCountFailedEvent
      {
        CourseId = context.Message.CourseId,
        ClassId = context.Message.ClassId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await context.Publish(new DecreaseClassEnrollmentsCountSuccessEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
      });
    }

  }

}
