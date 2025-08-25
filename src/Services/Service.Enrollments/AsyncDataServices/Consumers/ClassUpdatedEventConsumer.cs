using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

namespace Service.Enrollments.AsyncDataServices.Consumers;

public class ClassUpdatedEventConsumer : IConsumer<ClassUpdatedEvent>
{
  private readonly IMediator _mediator;

  public ClassUpdatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassUpdatedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByCourseCommand(context.Message.CourseId));
}
