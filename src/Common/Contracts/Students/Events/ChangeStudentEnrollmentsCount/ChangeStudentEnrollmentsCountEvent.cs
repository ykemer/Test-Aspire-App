using Contracts.Common;

namespace Contracts.Students.Events.ChangeStudentEnrollmentsCount;

public class ChangeStudentEnrollmentsCountEvent: Event
{
  public string StudentId { get; init; }
  public bool IsIncrease { get; init; }
}
