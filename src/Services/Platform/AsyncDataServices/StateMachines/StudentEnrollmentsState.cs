﻿using MassTransit;

namespace Platform.AsyncDataServices.StateMachines;

public class StudentEnrollmentsState : SagaStateMachineInstance
{
  public Guid CorrelationId { get; set; }
  public string CurrentState { get; set; }

  public Guid EventId { get; set; }
  public string StudentId { get; set; }
  public string CourseId { get; set; }
  public bool IsIncrease { get; set; }
  public bool IsStudentEnrollmentsUpdated { get; set; }
  public bool IsCourseEnrollmentsUpdated { get; set; }
  public DateTime EnrolledDate { get; set; }
  public string FailureReason { get; set; } = "";

}
