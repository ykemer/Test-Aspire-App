using System.Text.Json;
using Contracts.AsyncMessages;
using Contracts.Enrollments.Events;
using Contracts.Users.Events;
using Library.AsyncMessages;
using Service.Students.Features.CreateStudent;
using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }


    public async Task ProcessEvent(string message)
    {
        var eventType = AsyncMessageHelper.DetermineEvent(message);
        switch (eventType)
        {
            case AsyncEventType.UseCreated:
                await CreateUserProcessor(message);
                break;
            case AsyncEventType.StudentUnenrolled:
                await StudentUnenrollProcessor(message);
                break;
            case AsyncEventType.StudentEnrolled:
                await StudentEnrollProcessor(message);
                break;
        }
    }


    private async Task CreateUserProcessor(string publishedMessage)
    {
        var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(publishedMessage);
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new CreateStudentCommand
        {
            Id = userCreatedEvent.Id,
            FirstName = userCreatedEvent.FirstName,
            LastName = userCreatedEvent.LastName,
            Email = userCreatedEvent.Email,
            DateOfBirth = userCreatedEvent.DateOfBirth
        });
    }

    private async Task StudentUnenrollProcessor(string publishedMessage)
    {
        var studentEnrolledEvent = JsonSerializer.Deserialize<StudentEnrolledEvent>(publishedMessage);
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new UpdateStudentEnrollmentsCountCommand(studentEnrolledEvent.StudentId, false));
    }


    private async Task StudentEnrollProcessor(string publishedMessage)
    {
        var studentEnrolledEvent = JsonSerializer.Deserialize<StudentUnenrolledEvent>(publishedMessage);
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new UpdateStudentEnrollmentsCountCommand(studentEnrolledEvent.StudentId, true));
    }
}