﻿using Contracts.Users.Events;

using MassTransit;

using Service.Students.Features.CreateStudent;

namespace Service.Students.AsyncDataServices.Consumers;

public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
  private readonly IMediator _mediator;

  public UserCreatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<UserCreatedEvent> context)
  {
    var message = context.Message;
    await _mediator.Send(new CreateStudentCommand
    {
      Id = message.Id,
      FirstName = message.FirstName,
      LastName = message.LastName,
      Email = message.Email,
      DateOfBirth = message.DateOfBirth
    });
  }
}
