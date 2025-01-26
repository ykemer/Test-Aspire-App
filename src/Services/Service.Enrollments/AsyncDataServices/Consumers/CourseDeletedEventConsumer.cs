using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

namespace Service.Enrollments.AsyncDataServices.Consumers;

public class CourseDeletedEventConsumer : IConsumer<CourseDeletedEvent>
{
  private readonly IMediator _mediator;

  public CourseDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<CourseDeletedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByCourseCommand(context.Message.CourseId));
}
