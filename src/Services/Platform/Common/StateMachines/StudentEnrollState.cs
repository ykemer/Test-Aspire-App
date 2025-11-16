using MassTransit;

namespace Platform.Common.StateMachines;

public class StudentEnrollState : SagaStateMachineInstance
{
  public string CurrentState { get; set; }

  public Guid EventId { get; set; }
  public string StudentId { get; set; }
  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public bool IsStudentEnrollmentsUpdated { get; set; }
  public bool IsClassEnrollmentsUpdated { get; set; }
  public DateTime EnrolledDate { get; set; }
  public string FailureReason { get; set; } = "";
  public Guid CorrelationId { get; set; }
}
