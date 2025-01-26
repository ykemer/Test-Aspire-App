using Contracts.Enrollments.Events;

using MassTransit;

using Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices.Consumers;

public class StudentEnrolledConsumer : IConsumer<StudentEnrolledEvent>
{
  private readonly IMediator _mediator;

  public StudentEnrolledConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<StudentEnrolledEvent> context) =>
    await _mediator.Send(new IncreaseNumberOfEnrolledStudentsCommand(context.Message.CourseId));
}
