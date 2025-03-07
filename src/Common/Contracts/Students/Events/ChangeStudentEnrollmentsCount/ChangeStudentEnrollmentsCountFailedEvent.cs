using Contracts.Common;

namespace Contracts.Students.Events.ChangeStudentEnrollmentsCount;

public class ChangeStudentEnrollmentsCountFailedEvent: Event
{
  public string StudentId { get; set; }
  public string ErrorMessage { get; set; }
}
