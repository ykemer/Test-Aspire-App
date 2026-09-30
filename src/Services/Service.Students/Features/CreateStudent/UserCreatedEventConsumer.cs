using Contracts.Users.Events;

using Library.Messaging;

using Rebus.Handlers;

namespace Service.Students.Features.CreateStudent;

/// <summary>
/// Creates a student when a user registers in Platform. If that fails, the message is retried
/// (never silently lost), so no registered user ends up without a student record.
/// </summary>
public class UserCreatedEventConsumer : IHandleMessages<UserCreatedEvent>
{
  private readonly IMediator _mediator;

  public UserCreatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(UserCreatedEvent message)
  {
    var result = await _mediator.Send(message.ToCreateStudentCommand());
    result.ThrowIfFailed($"Creating student for user {message.Id}");
  }
}
