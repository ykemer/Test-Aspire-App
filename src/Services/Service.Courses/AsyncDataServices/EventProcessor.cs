using System.Text.Json;

using Contracts.AsyncMessages;
using Contracts.Enrollments.Events;

using Library.AsyncMessages;

using Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;
using Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

namespace Service.Courses.AsyncDataServices;

public class EventProcessor : IEventProcessor
{
  private readonly IServiceScopeFactory _scopeFactory;

  public EventProcessor(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;


  public async Task ProcessEvent(string message)
  {
    AsyncEventType eventType = AsyncMessageHelper.DetermineEvent(message);
    switch (eventType)
    {
      case AsyncEventType.StudentEnrolled:
        await IncreaseCourseEnrolledStudents(message);
        break;
      case AsyncEventType.StudentUnenrolled:
        await DecreaseCourseEnrolledStudents(message);
        break;
    }
  }


  private async Task IncreaseCourseEnrolledStudents(string publishedMessage)
  {
    StudentEnrolledEvent? studentEnrolledEvent = JsonSerializer.Deserialize<StudentEnrolledEvent>(publishedMessage);
    using IServiceScope? scope = _scopeFactory.CreateScope();
    IMediator? mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new IncreaseNumberOfEnrolledStudentsCommand(studentEnrolledEvent.CourseId));
  }

  private async Task DecreaseCourseEnrolledStudents(string publishedMessage)
  {
    StudentUnenrolledEvent? studentUnenrolledEvent =
      JsonSerializer.Deserialize<StudentUnenrolledEvent>(publishedMessage);
    using IServiceScope? scope = _scopeFactory.CreateScope();
    IMediator? mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new DecreaseNumberOfEnrolledStudentsCommand(studentUnenrolledEvent.CourseId));
  }
}
