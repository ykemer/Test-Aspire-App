using Contracts.Common;

namespace Contracts.Students.Events.ChangeStudentEnrollmentsCount;

public class ChangeStudentEnrollmentsCountSuccessEvent: Event
{
  public string StudentId { get; set; }
}
