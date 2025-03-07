using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

using MassTransit;

using Service.Courses.Features.Courses.UpdateNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices.Consumers;

public class ChangeCourseEnrollmentsCountEventConsumer : IConsumer<ChangeCourseEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;
  private readonly IPublishEndpoint _publishEndpoint;

  public ChangeCourseEnrollmentsCountEventConsumer(IMediator mediator, IPublishEndpoint publishEndpoint)
  {
    _mediator = mediator;
    _publishEndpoint = publishEndpoint;
  }

  public async Task Consume(ConsumeContext<ChangeCourseEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(context.Message.CourseId, context.Message.IsIncrease));
    if (result.IsError)
    {
      await _publishEndpoint.Publish(new ChangeCourseEnrollmentsCountFailedEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await _publishEndpoint.Publish(new ChangeCourseEnrollmentsCountSuccessEvent
      {
        CourseId = context.Message.CourseId,
        EventId = context.Message.EventId,
      });
    }

  }

}
