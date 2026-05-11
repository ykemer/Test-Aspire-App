using Rebus.Sagas;

namespace Platform.Common.StateMachines;

public class StudentEnrollState : ISagaData
{
  public Guid Id { get; set; }
  public int Revision { get; set; }

  public Guid EventId { get; set; }
  public Guid StudentId { get; set; }
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public bool IsStudentEnrollmentsUpdated { get; set; }
  public bool IsClassEnrollmentsUpdated { get; set; }
  public DateTime EnrolledDate { get; set; }
  public string FailureReason { get; set; } = "";
  public string State { get; set; } = "Initial";
}
