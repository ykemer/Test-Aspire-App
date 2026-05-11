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

public class StudentUnenrollStateMachine : Saga<StudentUnenrollState>,
  IAmInitiatedBy<EnrollmentDeletedEvent>,
  IHandleMessages<DecreaseStudentEnrollmentCountSuccessEvent>,
  IHandleMessages<DecreaseStudentEnrollmentCountFailedEvent>,
  IHandleMessages<DecreaseClassEnrollmentsCountSuccessEvent>,
  IHandleMessages<DecreaseClassEnrollmentsCountFailedEvent>
{
  private readonly IBus _bus;
  private readonly IHubContext<EnrollmentHub> _hubContext;

  public StudentUnenrollStateMachine(IBus bus, IHubContext<EnrollmentHub> hubContext)
  {
    _bus = bus;
    _hubContext = hubContext;
  }

  protected override void CorrelateMessages(ICorrelationConfig<StudentUnenrollState> config)
  {
    config.Correlate<EnrollmentDeletedEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<DecreaseStudentEnrollmentCountSuccessEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<DecreaseStudentEnrollmentCountFailedEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<DecreaseClassEnrollmentsCountSuccessEvent>(m => m.EventId, d => d.EventId);
    config.Correlate<DecreaseClassEnrollmentsCountFailedEvent>(m => m.EventId, d => d.EventId);
  }

  public async Task Handle(EnrollmentDeletedEvent message)
  {
    Data.StudentId = message.StudentId;
    Data.CourseId = message.CourseId;
    Data.ClassId = message.ClassId;
    Data.EventId = message.EventId;
    Data.EnrolledDate = DateTime.Now;
    Data.State = "ChangingStudentEnrollmentsCount";

    await _bus.Publish(new DecreaseStudentEnrollmentCountEvent
    {
      StudentId = Data.StudentId,
      EventId = Data.EventId
    });
  }

  public async Task Handle(DecreaseStudentEnrollmentCountSuccessEvent message)
  {
    if (Data.State != "ChangingStudentEnrollmentsCount") return;

    Data.IsStudentEnrollmentsUpdated = true;
    Data.State = "ChangingClassEnrollmentsCount";

    await _bus.Publish(new DecreaseClassEnrollmentsCountEvent
    {
      CourseId = Data.CourseId,
      ClassId = Data.ClassId,
      EventId = Data.EventId
    });
  }

  public async Task Handle(DecreaseStudentEnrollmentCountFailedEvent message)
  {
    Data.FailureReason = $"Student enrollments count update failed: {message.ErrorMessage}";
    Data.State = "Failed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
      "Failed to unenroll you from the class. Please contact support.");

    MarkAsComplete();
  }

  public async Task Handle(DecreaseClassEnrollmentsCountSuccessEvent message)
  {
    Data.IsClassEnrollmentsUpdated = true;
    Data.State = "Completed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentDeleted,
      "You have been successfully unenrolled from the class.");

    MarkAsComplete();
  }

  public async Task Handle(DecreaseClassEnrollmentsCountFailedEvent message)
  {
    Data.FailureReason = $"Class enrollments count update failed: {message.ErrorMessage}";
    Data.State = "Failed";

    await _hubContext.Clients.User(Data.StudentId.ToString()).SendAsync(
      EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
      "Failed to unenroll you from the class. Please contact support.");

    MarkAsComplete();
  }
}
