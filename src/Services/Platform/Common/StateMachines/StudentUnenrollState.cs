using MassTransit;

namespace Platform.Common.StateMachines;

public class StudentUnenrollState : SagaStateMachineInstance
{
  public string CurrentState { get; set; }

  public Guid EventId { get; set; }
  public Guid StudentId { get; set; }
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public bool IsStudentEnrollmentsUpdated { get; set; }
  public bool IsClassEnrollmentsUpdated { get; set; }
  public DateTime EnrolledDate { get; set; }
  public string FailureReason { get; set; } = "";
  public Guid CorrelationId { get; set; }
}
