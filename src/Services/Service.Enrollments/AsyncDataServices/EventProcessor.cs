using System.Text.Json;

using Contracts.AsyncMessages;
using Contracts.Courses.Events;
using Contracts.Students.Events;

using Library.AsyncMessages;

using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;
using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByStudent;

namespace Service.Enrollments.AsyncDataServices;

public class EventProcessor : IEventProcessor
{
  private readonly IServiceScopeFactory _scopeFactory;

  public EventProcessor(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;


  public async Task ProcessEvent(string message)
  {
    AsyncEventType eventType = AsyncMessageHelper.DetermineEvent(message);
    switch (eventType)
    {
      case AsyncEventType.CourseDeleted:
        await CourseDeletedProcessor(message);
        break;
      case AsyncEventType.StudentDeleted:
        await StudentDeletedProcessor(message);
        break;
    }
  }


  private async Task CourseDeletedProcessor(string publishedMessage)
  {
    CourseDeletedEvent? courseDeletedEvent = JsonSerializer.Deserialize<CourseDeletedEvent>(publishedMessage);
    using IServiceScope? scope = _scopeFactory.CreateScope();
    IMediator? mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new DeleteEnrollmentsByCourseCommand(courseDeletedEvent.CourseId));
  }

  private async Task StudentDeletedProcessor(string publishedMessage)
  {
    StudentDeletedEvent? studentDeletedEvent = JsonSerializer.Deserialize<StudentDeletedEvent>(publishedMessage);
    using IServiceScope? scope = _scopeFactory.CreateScope();
    IMediator? mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new DeleteEnrollmentsByStudentCommand(studentDeletedEvent.StudentId));
  }
}
