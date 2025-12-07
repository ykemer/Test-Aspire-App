using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;
using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Features.Enrollments;

namespace Platform.Common.StateMachines;

public class StudentUnenrollStateMachine : MassTransitStateMachine<StudentUnenrollState>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;


  public StudentUnenrollStateMachine(IHubContext<EnrollmentHub> hubContext)
  {
    _hubContext = hubContext;

    InstanceState(x => x.CurrentState);

    Event(() => EnrollmentDeleted, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => StudentEnrollmentCountChanged, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => DecreaseStudentEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => DecreaseStudentEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => DecreaseClassEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => DecreaseClassEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));


    Initially(
      When(EnrollmentDeleted)
        .Then(context =>
        {
          context.Saga.StudentId = context.Message.StudentId;
          context.Saga.CourseId = context.Message.CourseId;
          context.Saga.ClassId = context.Message.ClassId;
          context.Saga.EventId = context.Message.EventId;
          context.Saga.EnrolledDate = DateTime.Now;
        })
        .TransitionTo(ChangingStudentEnrollmentsCount)
        .Publish(context => new DecreaseStudentEnrollmentCountEvent
        {
          StudentId = context.Saga.StudentId, EventId = context.Saga.EventId
        })
    );


    During(ChangingStudentEnrollmentsCount,
      When(DecreaseStudentEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Saga.IsStudentEnrollmentsUpdated = true;
        })
        .Publish(context => new DecreaseClassEnrollmentsCountEvent
        {
          CourseId = context.Saga.CourseId, ClassId = context.Saga.ClassId, EventId = context.Saga.EventId
        })
        .TransitionTo(ChangingClassEnrollmentsCount),
      When(DecreaseStudentEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Student enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(
            EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
            "Failed to unenroll you from the class. Please contact support.");
        })
        .TransitionTo(Failed)
    );

    During(ChangingClassEnrollmentsCount,
      When(DecreaseClassEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Class enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(
            EnrollmentHubMessages.EnrollmentDeleteRequestRejected,
            "Failed to unenroll you from the class. Please contact support.");
        })
        .Publish(context => new DecreaseStudentEnrollmentCountEvent
        {
          StudentId = context.Saga.StudentId, EventId = context.Saga.EventId
        })
        .TransitionTo(Failed)
    );

    DuringAny(
      When(DecreaseClassEnrollmentsCountSuccess)
        .ThenAsync(async context =>
        {
          context.Saga.IsClassEnrollmentsUpdated = true;
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentDeleted,
            "You have been successfully unenrolled from the class.");
        })
        .TransitionTo(Completed)
        .Finalize());

    SetCompletedWhenFinalized();
  }

  public Event<EnrollmentDeletedEvent> EnrollmentDeleted { get; set; }
  public Event<StudentEnrollmentCountChangedEvent> StudentEnrollmentCountChanged { get; set; }
  public Event<DecreaseStudentEnrollmentCountSuccessEvent> DecreaseStudentEnrollmentsCountSuccess { get; set; }
  public Event<DecreaseStudentEnrollmentCountFailedEvent> DecreaseStudentEnrollmentsCountFailed { get; set; }

  public Event<DecreaseClassEnrollmentsCountFailedEvent> DecreaseClassEnrollmentsCountFailed { get; set; }
  public Event<DecreaseClassEnrollmentsCountSuccessEvent> DecreaseClassEnrollmentsCountSuccess { get; set; }


  public State ChangingStudentEnrollmentsCount { get; set; }
  public State ChangingClassEnrollmentsCount { get; set; }
  public State Completed { get; set; }
  public State Failed { get; set; }
}
