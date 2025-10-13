using Contracts.Students.Events;

using MassTransit;

using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByStudent;

namespace Service.Enrollments.Common.AsyncDataServices.Consumers;

public class StudentDeletedEventConsumer : IConsumer<StudentDeletedEvent>
{
  private readonly IMediator _mediator;

  public StudentDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<StudentDeletedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByStudentCommand(context.Message.StudentId));
}
