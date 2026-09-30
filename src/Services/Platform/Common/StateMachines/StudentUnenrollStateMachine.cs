using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;
using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using Microsoft.AspNetCore.SignalR;

using Platform.Features.Enrollments;

using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace Platform.Common.StateMachines;

/// <summary>
/// After the Enrollments service removed an enrollment, this saga updates the counters in the other services:
/// <list type="number">
/// <item>Students: -1 enrollment for the student.</item>
/// <item>Courses: -1 student for the class (and its course).</item>
/// <item>Tells the student the result through SignalR.</item>
/// </list>
/// Known gap: if step 2 fails after step 1 succeeded, nothing is undone (no compensation yet).
/// </summary>
public class StudentUnenrollStateMachine : Saga<StudentUnenrollState>,
  IAmInitiatedBy<EnrollmentDeletedEvent>,
  IHandleMessages<DecreaseStudentEnrollmentCountSuccessEvent>,
  IHandleMessages<DecreaseStudentEnrollmentCountFailedEvent>,
  IHandleMessages<DecreaseClassEnrollmentsCountSuccessEvent>,
  IHandleMessages<DecreaseClassEnrollmentsCountFailedEvent>
{
  private const string FailedMessage = "Failed to unenroll you from the class. Please contact support.";
  private const string SucceededMessage = "You have been successfully unenrolled from the class.";

  private readonly IBus _bus;
  private readonly IHubContext<EnrollmentHub> _hubContext;
  private readonly TimeProvider _timeProvider;

  public StudentUnenrollStateMachine(IBus bus, IHubContext<EnrollmentHub> hubContext, TimeProvider timeProvider)
  {
    _bus = bus;
    _hubContext = hubContext;
    _timeProvider = timeProvider;
  }

  protected override void CorrelateMessages(ICorrelationConfig<StudentUnenrollState> config)
  {
    config.Correlate<EnrollmentDeletedEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<DecreaseStudentEnrollmentCountSuccessEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<DecreaseStudentEnrollmentCountFailedEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<DecreaseClassEnrollmentsCountSuccessEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<DecreaseClassEnrollmentsCountFailedEvent>(message => message.EventId, data => data.EventId);
  }

  public async Task Handle(EnrollmentDeletedEvent message)
  {
    Data.StudentId = message.StudentId;
    Data.CourseId = message.CourseId;
    Data.ClassId = message.ClassId;
    Data.EventId = message.EventId;
    Data.StartedAt = _timeProvider.GetUtcNow().UtcDateTime;
    Data.State = SagaStates.ChangingStudentEnrollmentsCount;

    await _bus.Publish(new DecreaseStudentEnrollmentCountEvent { StudentId = Data.StudentId, EventId = Data.EventId });
  }

  public async Task Handle(DecreaseStudentEnrollmentCountSuccessEvent message)
  {
    if (Data.State != SagaStates.ChangingStudentEnrollmentsCount)
    {
      return; // a repeated message: this step is already done
    }

    Data.IsStudentEnrollmentsUpdated = true;
    Data.State = SagaStates.ChangingClassEnrollmentsCount;

    await _bus.Publish(new DecreaseClassEnrollmentsCountEvent
    {
      CourseId = Data.CourseId, ClassId = Data.ClassId, EventId = Data.EventId
    });
  }

  public Task Handle(DecreaseStudentEnrollmentCountFailedEvent message) =>
    Fail($"Student enrollments count update failed: {message.ErrorMessage}");

  public async Task Handle(DecreaseClassEnrollmentsCountSuccessEvent message)
  {
    Data.IsClassEnrollmentsUpdated = true;
    Data.State = SagaStates.Completed;

    await NotifyStudent(EnrollmentHubMessages.EnrollmentDeleted, SucceededMessage);
    MarkAsComplete();
  }

  public Task Handle(DecreaseClassEnrollmentsCountFailedEvent message) =>
    Fail($"Class enrollments count update failed: {message.ErrorMessage}");

  private async Task Fail(string reason)
  {
    Data.FailureReason = reason;
    Data.State = SagaStates.Failed;

    await NotifyStudent(EnrollmentHubMessages.EnrollmentDeleteRequestRejected, FailedMessage);
    MarkAsComplete();
  }

  private Task NotifyStudent(string hubMessage, string text) =>
    _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(hubMessage, text);
}
