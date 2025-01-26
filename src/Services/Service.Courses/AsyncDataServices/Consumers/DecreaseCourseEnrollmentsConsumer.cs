using Contracts.Courses.Events;

using MassTransit;

using Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices.Consumers;

public class DecreaseCourseEnrollmentsConsumer: IConsumer<DecreaseCourseEnrollments>
{
  private readonly IMediator _mediator;

  public DecreaseCourseEnrollmentsConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<DecreaseCourseEnrollments> context)
  {
    await _mediator.Send(new DecreaseNumberOfEnrolledStudentsCommand(context.Message.CourseId));
  }
}
