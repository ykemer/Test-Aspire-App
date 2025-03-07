using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeCourseEnrollmentsCountEvent: Event
{
  public string CourseId { get; set; }
  public bool IsIncrease { get; set; }
}
