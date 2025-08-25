using Contracts.Courses.Events.ChangeCourseEnrollmentsCount;
using Contracts.Enrollments.Events;
using Contracts.Students.Events.ChangeStudentEnrollmentsCount;

using MassTransit;

namespace Platform.AsyncDataServices.StateMachines;

public class StudentEnrollmentsStateMachine : MassTransitStateMachine<StudentEnrollmentsState>
{
  public Event<StudentEnrollmentCountChangedEvent> StudentEnrollmentCountChanged { get; set; }
  public Event<ChangeStudentEnrollmentsCountSuccessEvent> ChangeStudentEnrollmentsCountSuccess { get; set; }
  public Event<ChangeStudentEnrollmentsCountFailedEvent> ChangeStudentEnrollmentsCountFailed { get; set; }

  public Event<ChangeClassEnrollmentsCountFailedEvent> ChangeCourseEnrollmentsCountFailed { get; set; }
  public Event<ChangeClassEnrollmentsCountSuccessEvent> ChangeCourseEnrollmentsCountSuccess { get; set; }


  public State ChangingStudentEnrollmentsCount { get; set; }
  public State ChangingCourseEnrollmentsCount { get; set; }
  public State Completed { get; set; }
  public State Failed { get; set; }


  public StudentEnrollmentsStateMachine()
  {
    InstanceState(x => x.CurrentState);

    Event(() => StudentEnrollmentCountChanged, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => ChangeStudentEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => ChangeStudentEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));

    Event(() => ChangeCourseEnrollmentsCountFailed, x => x.CorrelateById(m => m.Message.EventId));
    Event(() => ChangeCourseEnrollmentsCountSuccess, x => x.CorrelateById(m => m.Message.EventId));

    Initially(
      When(StudentEnrollmentCountChanged)
        .Then(context =>
        {
          context.Saga.StudentId = context.Message.StudentId;
          context.Saga.CourseId = context.Message.CourseId;
          context.Saga.ClassId = context.Message.ClassId;
          context.Saga.EventId = context.Message.EventId;
          context.Saga.IsIncrease = context.Message.IsIncrease;
          context.Saga.EnrolledDate = DateTime.Now;
        })
        .TransitionTo(ChangingStudentEnrollmentsCount)
        .Publish(context => new ChangeStudentEnrollmentsCountEvent
        {
          StudentId = context.Saga.StudentId,
          EventId = context.Saga.EventId,
          IsIncrease = context.Saga.IsIncrease
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
        .Then(context =>
        {
          context.Saga.FailureReason = $"Student enrollments count update failed: {context.Message.ErrorMessage}";
        })
        .TransitionTo(Failed)
        .Finalize()
    );

    During(ChangingCourseEnrollmentsCount,
      When(ChangeCourseEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Saga.IsCourseEnrollmentsUpdated = true;
        })
        .TransitionTo(Completed)
        .Finalize(),
      When(ChangeCourseEnrollmentsCountFailed)
        .Then(context =>
        {
          context.Saga.FailureReason = $"Course enrollments count update failed: {context.Message.ErrorMessage}";
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
