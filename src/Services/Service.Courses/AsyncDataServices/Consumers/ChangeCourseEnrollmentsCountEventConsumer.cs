using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

using MassTransit;

using Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices.Consumers;

public class ChangeCourseEnrollmentsCountEventConsumer : IConsumer<ChangeClassEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;
  private readonly IPublishEndpoint _publishEndpoint;

  public ChangeCourseEnrollmentsCountEventConsumer(IMediator mediator, IPublishEndpoint publishEndpoint)
  {
    _mediator = mediator;
    _publishEndpoint = publishEndpoint;
  }

  public async Task Consume(ConsumeContext<ChangeClassEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(context.Message.CourseId, context.Message.ClassId, context.Message.IsIncrease));
    if (result.IsError)
    {
      await _publishEndpoint.Publish(new ChangeClassEnrollmentsCountFailedEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await _publishEndpoint.Publish(new ChangeClassEnrollmentsCountSuccessEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
      });
    }

  }

}
