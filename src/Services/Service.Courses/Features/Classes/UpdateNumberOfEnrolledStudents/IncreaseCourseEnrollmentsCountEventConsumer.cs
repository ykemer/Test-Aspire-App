using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

using MassTransit;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public class IncreaseCourseEnrollmentsCountEventConsumer : IConsumer<IncreaseClassEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;

  public IncreaseCourseEnrollmentsCountEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<IncreaseClassEnrollmentsCountEvent> context)
  {
    var result =
      await _mediator.Send(
        new UpdateNumberOfEnrolledStudentsCommand(context.Message.CourseId, context.Message.ClassId, true));
    if (result.IsError)
    {
      await context.Publish(new IncreaseClassEnrollmentsCountFailedEvent
      {
        CourseId = context.Message.CourseId,
        ClassId = context.Message.ClassId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await context.Publish(new IncreaseClassEnrollmentsCountSuccessEvent
      {
        CourseId = context.Message.CourseId, EventId = context.Message.EventId
      });
    }
  }
}
