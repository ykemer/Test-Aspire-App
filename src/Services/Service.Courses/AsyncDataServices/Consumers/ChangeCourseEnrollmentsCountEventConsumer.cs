using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

using MassTransit;

using Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices.Consumers;

public class ChangeCourseEnrollmentsCountEventConsumer : IConsumer<ChangeClassEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;

  public ChangeCourseEnrollmentsCountEventConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<ChangeClassEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(context.Message.CourseId, context.Message.ClassId, context.Message.IsIncrease));
    if (result.IsError)
    {
      await context.Publish(new ChangeClassEnrollmentsCountFailedEvent
      {
        CourseId = context.Message.CourseId,
        ClassId = context.Message.ClassId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await context.Publish(new ChangeClassEnrollmentsCountSuccessEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
      });
    }

  }

}
