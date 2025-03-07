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

  public Event<ChangeCourseEnrollmentsCountFailedEvent> ChangeCourseEnrollmentsCountFailed { get; set; }
  public Event<ChangeCourseEnrollmentsCountSuccessEvent> ChangeCourseEnrollmentsCountSuccess { get; set; }


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
          context.Instance.StudentId = context.Data.StudentId;
          context.Instance.CourseId = context.Data.CourseId;
          context.Instance.EventId = context.Data.EventId;
          context.Instance.IsIncrease = context.Data.IsIncrease;
          context.Instance.EnrolledDate = DateTime.Now;
        })
        .TransitionTo(ChangingStudentEnrollmentsCount)
        .Publish(context => new ChangeStudentEnrollmentsCountEvent
        {
          StudentId = context.Instance.StudentId,
          EventId = context.Instance.EventId,
          IsIncrease = context.Instance.IsIncrease
        })
    );

    During(ChangingStudentEnrollmentsCount,
      When(ChangeStudentEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Instance.IsStudentEnrollmentsUpdated = true;
        })
        .Publish(context => new ChangeCourseEnrollmentsCountEvent
        {
          CourseId = context.Instance.CourseId,
          EventId = context.Instance.EventId,
          IsIncrease = context.Instance.IsIncrease
        })
        .TransitionTo(ChangingCourseEnrollmentsCount),
      When(ChangeStudentEnrollmentsCountFailed)
        .Then(context =>
        {
          context.Instance.FailureReason = $"Student enrollments count update failed: {context.Data.ErrorMessage}";
        })
        .TransitionTo(Failed)
        .Finalize()
    );

    During(ChangingCourseEnrollmentsCount,
      When(ChangeCourseEnrollmentsCountSuccess)
        .Then(context =>
        {
          context.Instance.IsCourseEnrollmentsUpdated = true;
        })
        .TransitionTo(Completed)
        .Finalize(),
      When(ChangeCourseEnrollmentsCountFailed)
        .Then(context =>
        {
          context.Instance.FailureReason = $"Course enrollments count update failed: {context.Data.ErrorMessage}";
        })
        .Publish(context => new ChangeStudentEnrollmentsCountEvent
        {
          StudentId = context.Instance.StudentId,
          EventId = context.Instance.EventId,
          IsIncrease = !context.Instance.IsIncrease
        })
        .TransitionTo(Failed)
        .Finalize()
    );

    SetCompletedWhenFinalized();
  }
}
