using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Enrollments.Hub;
using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Features.Enrollments;

namespace Platform.Common.StateMachines;

public class StudentEnrollStateMachine : MassTransitStateMachine<StudentEnrollState>
{
  private readonly IHubContext<EnrollmentHub> _hubContext;


  public StudentEnrollStateMachine(IHubContext<EnrollmentHub> hubContext)
  {
    _hubContext = hubContext;

    InstanceState(x => x.CurrentState);

    Event(() => EnrollmentCreated, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => StudentEnrollmentCountChanged, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => IncreaseStudentEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => IncreaseStudentEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => IncreaseClassEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => IncreaseClassEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));


    Initially(
      When(EnrollmentCreated)
        .Then(context =>
        {
          context.Saga.StudentId = context.Message.StudentId;
          context.Saga.CourseId = context.Message.CourseId;
          context.Saga.ClassId = context.Message.ClassId;
          context.Saga.EventId = context.Message.EventId;
          context.Saga.EnrolledDate = DateTime.Now;
        })
        .TransitionTo(ChangingStudentEnrollmentsCount)
        .Publish(context => new IncreaseStudentEnrollmentsCountEvent
        {
          StudentId = context.Saga.StudentId, EventId = context.Saga.EventId
        })
    );


    During(ChangingStudentEnrollmentsCount,
      When(IncreaseStudentEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Saga.IsStudentEnrollmentsUpdated = true;
        })
        .Publish(context => new IncreaseClassEnrollmentsCountEvent
        {
          CourseId = context.Saga.CourseId, ClassId = context.Saga.ClassId, EventId = context.Saga.EventId
        })
        .TransitionTo(ChangingClassEnrollmentsCount),
      When(IncreaseStudentEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Student enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(
            EnrollmentHubMessages.EnrollmentCreateRequestRejected,
            "Failed to enroll you in the class. Please contact support.");
        })
        .TransitionTo(Failed)
    );

    During(ChangingClassEnrollmentsCount,
      When(IncreaseClassEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Class enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(
            EnrollmentHubMessages.EnrollmentCreateRequestRejected,
            "Failed to enroll you in the class. Please contact support.");
        })
        .Publish(context => new IncreaseStudentEnrollmentsCountEvent
        {
          StudentId = context.Saga.StudentId, EventId = context.Saga.EventId
        })
        .TransitionTo(Failed)
    );


    DuringAny(
      When(IncreaseClassEnrollmentsCountSuccess)
        .ThenAsync(async context =>
        {
          context.Saga.IsClassEnrollmentsUpdated = true;
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentCreated,
            "You have been successfully enrolled in the class.");
        })
        .TransitionTo(Completed)
        .Finalize());

    SetCompletedWhenFinalized();
  }


  public Event<EnrollmentCreatedEvent> EnrollmentCreated { get; set; }
  public Event<StudentEnrollmentCountChangedEvent> StudentEnrollmentCountChanged { get; set; }
  public Event<IncreaseStudentEnrollmentsCountSuccessEvent> IncreaseStudentEnrollmentsCountSuccess { get; set; }
  public Event<IncreaseStudentEnrollmentsCountFailedEvent> IncreaseStudentEnrollmentsCountFailed { get; set; }

  public Event<IncreaseClassEnrollmentsCountFailedEvent> IncreaseClassEnrollmentsCountFailed { get; set; }
  public Event<IncreaseClassEnrollmentsCountSuccessEvent> IncreaseClassEnrollmentsCountSuccess { get; set; }


  public State ChangingStudentEnrollmentsCount { get; set; }
  public State ChangingClassEnrollmentsCount { get; set; }
  public State Completed { get; set; }
  public State Failed { get; set; }
}

// TODO: delete enrollment from enrollment service in case of failure
