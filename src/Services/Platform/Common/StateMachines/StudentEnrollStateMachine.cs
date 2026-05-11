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

public class StudentEnrollStateMachine : Saga<StudentEnrollState>,
  IAmInitiatedBy<EnrollmentCreatedEvent>,
  IHandleMessages<IncreaseStudentEnrollmentsCountSuccessEvent>,
  IHandleMessages<IncreaseStudentEnrollmentsCountFailedEvent>,
  IHandleMessages<IncreaseClassEnrollmentsCountSuccessEvent>,
  IHandleMessages<IncreaseClassEnrollmentsCountFailedEvent>
{
  private readonly IBus _bus;
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public StudentEnrollStateMachine(IBus bus, IHubContext<EnrollmentHub> hubContext)
  {
    _bus = bus;
    _hubContext = hubContext;
  }

  protected override void CorrelateMessages(ICorrelationConfig<StudentEnrollState> config)
  {
    config.Correlate<EnrollmentCreatedEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<IncreaseStudentEnrollmentsCountSuccessEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<IncreaseStudentEnrollmentsCountFailedEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<IncreaseClassEnrollmentsCountSuccessEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<IncreaseClassEnrollmentsCountFailedEvent>(m => m.EventId, d => d.EventId);
  }

  public async Task Handle(EnrollmentCreatedEvent message)
  {
    Data.StudentId = message.StudentId;
    Data.CourseId = message.CourseId;
    Data.ClassId = message.ClassId;
    Data.EventId = message.EventId;
    Data.EnrolledDate = DateTime.Now;
    Data.State = "ChangingStudentEnrollmentsCount";

    await _bus.Publish(new IncreaseStudentEnrollmentsCountEvent
    {
      StudentId = Data.StudentId,
      EventId = Data.EventId
    });
  }

  public async Task Handle(IncreaseStudentEnrollmentsCountSuccessEvent message)
  {
    if (Data.State != "ChangingStudentEnrollmentsCount") return;

    Data.IsStudentEnrollmentsUpdated = true;
    Data.State = "ChangingClassEnrollmentsCount";

    await _bus.Publish(new IncreaseClassEnrollmentsCountEvent
    {
      CourseId = Data.CourseId,
      ClassId = Data.ClassId,
      EventId = Data.EventId
    });
  }

  public async Task Handle(IncreaseStudentEnrollmentsCountFailedEvent message)
  {
    Data.FailureReason = $"Student enrollments count update failed: {message.ErrorMessage}";
    Data.State = "Failed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentCreateRequestRejected,
      "Failed to enroll you in the class. Please contact support.");

    MarkAsComplete();
  }

  public async Task Handle(IncreaseClassEnrollmentsCountSuccessEvent message)
  {
    Data.IsClassEnrollmentsUpdated = true;
    Data.State = "Completed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentCreated,
      "You have been successfully enrolled in the class.");

    MarkAsComplete();
  }

  public async Task Handle(IncreaseClassEnrollmentsCountFailedEvent message)
  {
    Data.FailureReason = $"Class enrollments count update failed: {message.ErrorMessage}";
    Data.State = "Failed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentCreateRequestRejected,
      "Failed to enroll you in the class. Please contact support.");

    MarkAsComplete();
  }
}
