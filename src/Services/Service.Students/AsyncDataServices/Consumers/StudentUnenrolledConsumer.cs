using Contracts.Enrollments.Events;

using MassTransit;

using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices.Consumers;

public class StudentUnenrolledConsumer : IConsumer<StudentUnenrolledEvent>
{
  private readonly IMediator _mediator;

  public StudentUnenrolledConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<StudentUnenrolledEvent> context) =>
    await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, false));
}
