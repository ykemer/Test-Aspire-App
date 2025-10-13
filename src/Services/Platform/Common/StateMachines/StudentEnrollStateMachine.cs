using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Hub;
using Contracts.Students.Events.ChangeStudentEnrollmentsCount;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Common.StateMachines;

public class StudentEnrollStateMachine : MassTransitStateMachine<StudentEnrollState>
{


  public Event<EnrollmentCreatedEvent> EnrollmentCreated { get; set; }
  public Event<StudentEnrollmentCountChangedEvent> StudentEnrollmentCountChanged { get; set; }
  public Event<ChangeStudentEnrollmentsCountSuccessEvent> ChangeStudentEnrollmentsCountSuccess { get; set; }
  public Event<ChangeStudentEnrollmentsCountFailedEvent> ChangeStudentEnrollmentsCountFailed { get; set; }

  public Event<ChangeClassEnrollmentsCountFailedEvent> ChangeCourseEnrollmentsCountFailed { get; set; }
  public Event<ChangeClassEnrollmentsCountSuccessEvent> ChangeCourseEnrollmentsCountSuccess { get; set; }


  public State ChangingStudentEnrollmentsCount { get; set; }
  public State ChangingCourseEnrollmentsCount { get; set; }
  public State Completed { get; set; }
  public State Failed { get; set; }

  private readonly IHubContext<EnrollmentHub> _hubContext;


  public StudentEnrollStateMachine(IHubContext<EnrollmentHub> hubContext)
  {
    _hubContext = hubContext;

    InstanceState(x => x.CurrentState);

    Event(() => EnrollmentCreated, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => StudentEnrollmentCountChanged, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => ChangeStudentEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => ChangeStudentEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => ChangeCourseEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => ChangeCourseEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));


    Initially(
      When(EnrollmentCreated)
        .Then(context =>
        {
          context.Saga.StudentId = context.Message.StudentId;
          context.Saga.CourseId = context.Message.CourseId;
          context.Saga.ClassId = context.Message.ClassId;
          context.Saga.EventId = context.Message.EventId;
          context.Saga.IsIncrease = true;
          context.Saga.EnrolledDate = DateTime.Now;
        })
        .TransitionTo(ChangingStudentEnrollmentsCount)
        .Publish(context => new ChangeStudentEnrollmentsCountEvent
        {
          StudentId = context.Saga.StudentId,
          IsIncrease = context.Saga.IsIncrease,
          EventId = context.Saga.EventId,
        })
    );


    During(ChangingStudentEnrollmentsCount,
      When(ChangeStudentEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Saga.IsStudentEnrollmentsUpdated = true;
        })
        .Publish(context => new ChangeClassEnrollmentsCountEvent
        {
          CourseId = context.Saga.CourseId,
          ClassId = context.Saga.ClassId,
          EventId = context.Saga.EventId,
          IsIncrease = context.Saga.IsIncrease
        })
        .TransitionTo(ChangingCourseEnrollmentsCount),
      When(ChangeStudentEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Student enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentCreateRequestRejected, $"Failed to enroll you in the course. Please contact support.");
        })
        .TransitionTo(Failed)
        .Finalize()
    );

    During(ChangingCourseEnrollmentsCount,
      When(ChangeCourseEnrollmentsCountSuccess)
        .ThenAsync(async(context) =>
        {
          context.Saga.IsCourseEnrollmentsUpdated = true;
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentCreated, $"You have been successfully enrolled in the course.");
        })
        .TransitionTo(Completed)
        .Finalize(),
      When(ChangeCourseEnrollmentsCountFailed)
        .ThenAsync(async context =>
        {
          context.Saga.FailureReason = $"Course enrollments count update failed: {context.Message.ErrorMessage}";
          await _hubContext.Clients.User(context.Saga.StudentId).SendAsync(EnrollmentHubMessages.EnrollmentCreateRequestRejected, $"Failed to enroll you in the course. Please contact support.");
        })
        .Publish(context => new ChangeStudentEnrollmentsCountEvent
        {
          StudentId = context.Saga.StudentId,
          EventId = context.Saga.EventId,
          IsIncrease = !context.Saga.IsIncrease
        })
        .TransitionTo(Failed)
        .Finalize()
    );

    SetCompletedWhenFinalized();
  }
}


// TODO: delete enrollment from enrollment service in case of failure
