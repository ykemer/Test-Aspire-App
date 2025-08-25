using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Classes.UpdateClass;
using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

namespace Service.Enrollments.AsyncDataServices.Consumers;

public class ClassUpdatedEventConsumer : IConsumer<ClassUpdatedEvent>
{
  private readonly IMediator _mediator;

  public ClassUpdatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassUpdatedEvent> context) =>
    await _mediator.Send(new UpdateClassCommand
    {
      MaxStudents = context.Message.MaxStudents,
      CourseEndDate = context.Message.CourseEndDate,
      CourseStartDate = context.Message.CourseStartDate,
      RegistrationDeadline = context.Message.RegistrationDeadline,
      CourseId = context.Message.CourseId,
      Id = context.Message.Id,
    });
}
