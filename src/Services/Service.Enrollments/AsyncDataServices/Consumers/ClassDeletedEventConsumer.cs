﻿using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Classes.DeleteEnrollmentsByClass;
using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

namespace Service.Enrollments.AsyncDataServices.Consumers;

public class ClassDeletedEventConsumer : IConsumer<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassDeletedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByClassCommand(context.Message.CourseId, context.Message.ClassId));
}
