﻿using Contracts.Courses.Events;

using MassTransit;

namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

public class CourseDeletedEventConsumer : IConsumer<CourseDeletedEvent>
{
  private readonly IMediator _mediator;

  public CourseDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<CourseDeletedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByCourseCommand(context.Message.CourseId));
}
