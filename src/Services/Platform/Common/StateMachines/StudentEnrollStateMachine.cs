using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using Microsoft.AspNetCore.SignalR;

using Platform.Features.Enrollments;

using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace Platform.Common.StateMachines;

/// <summary>
/// After the Enrollments service saved a new enrollment, this saga updates the counters in the other services:
/// <list type="number">
/// <item>Students: +1 enrollment for the student.</item>
/// <item>Courses: +1 student for the class (and its course).</item>
/// <item>Tells the student the result through SignalR.</item>
/// </list>
/// Known gap: if step 2 fails after step 1 succeeded, nothing is undone (no compensation yet), so the
/// student's counter stays one too high. Closing it needs a "compensate" message in the Students service.
/// </summary>
public class StudentEnrollStateMachine : Saga<StudentEnrollState>,
  IAmInitiatedBy<EnrollmentCreatedEvent>,
  IHandleMessages<IncreaseStudentEnrollmentsCountSuccessEvent>,
  IHandleMessages<IncreaseStudentEnrollmentsCountFailedEvent>,
  IHandleMessages<IncreaseClassEnrollmentsCountSuccessEvent>,
  IHandleMessages<IncreaseClassEnrollmentsCountFailedEvent>
{
  private const string FailedMessage = "Failed to enroll you in the class. Please contact support.";
  private const string SucceededMessage = "You have been successfully enrolled in the class.";

  private readonly IBus _bus;
  private readonly IHubContext<EnrollmentHub> _hubContext;
  private readonly TimeProvider _timeProvider;

  public StudentEnrollStateMachine(IBus bus, IHubContext<EnrollmentHub> hubContext, TimeProvider timeProvider)
  {
    _bus = bus;
    _hubContext = hubContext;
    _timeProvider = timeProvider;
  }

  protected override void CorrelateMessages(ICorrelationConfig<StudentEnrollState> config)
  {
    config.Correlate<EnrollmentCreatedEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<IncreaseStudentEnrollmentsCountSuccessEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<IncreaseStudentEnrollmentsCountFailedEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<IncreaseClassEnrollmentsCountSuccessEvent>(message => message.EventId, data => data.EventId);
    config.Correlate<IncreaseClassEnrollmentsCountFailedEvent>(message => message.EventId, data => data.EventId);
  }

  public async Task Handle(EnrollmentCreatedEvent message)
  {
    Data.StudentId = message.StudentId;
    Data.CourseId = message.CourseId;
    Data.ClassId = message.ClassId;
    Data.EventId = message.EventId;
    Data.StartedAt = _timeProvider.GetUtcNow().UtcDateTime;
    Data.State = SagaStates.ChangingStudentEnrollmentsCount;

    await _bus.Publish(new IncreaseStudentEnrollmentsCountEvent { StudentId = Data.StudentId, EventId = Data.EventId });
  }

  public async Task Handle(IncreaseStudentEnrollmentsCountSuccessEvent message)
  {
    if (Data.State != SagaStates.ChangingStudentEnrollmentsCount)
    {
      return; // a repeated message: this step is already done
    }

    Data.IsStudentEnrollmentsUpdated = true;
    Data.State = SagaStates.ChangingClassEnrollmentsCount;

    await _bus.Publish(new IncreaseClassEnrollmentsCountEvent
    {
      CourseId = Data.CourseId, ClassId = Data.ClassId, EventId = Data.EventId
    });
  }

  public Task Handle(IncreaseStudentEnrollmentsCountFailedEvent message) =>
    Fail($"Student enrollments count update failed: {message.ErrorMessage}");

  public async Task Handle(IncreaseClassEnrollmentsCountSuccessEvent message)
  {
    Data.IsClassEnrollmentsUpdated = true;
    Data.State = SagaStates.Completed;

    await NotifyStudent(EnrollmentHubMessages.EnrollmentCreated, SucceededMessage);
    MarkAsComplete();
  }

  public Task Handle(IncreaseClassEnrollmentsCountFailedEvent message) =>
    Fail($"Class enrollments count update failed: {message.ErrorMessage}");

  private async Task Fail(string reason)
  {
    Data.FailureReason = reason;
    Data.State = SagaStates.Failed;

    await NotifyStudent(EnrollmentHubMessages.EnrollmentCreateRequestRejected, FailedMessage);
    MarkAsComplete();
  }

  private Task NotifyStudent(string hubMessage, string text) =>
    _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(hubMessage, text);
}
