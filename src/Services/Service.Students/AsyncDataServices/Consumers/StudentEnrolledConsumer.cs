using Contracts.Enrollments.Events;

using MassTransit;

using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices.Consumers;

public class StudentEnrolledConsumer : IConsumer<StudentEnrolledEvent>
{
  private readonly IMediator _mediator;

  public StudentEnrolledConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<StudentEnrolledEvent> context) =>
    await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, true));
}
