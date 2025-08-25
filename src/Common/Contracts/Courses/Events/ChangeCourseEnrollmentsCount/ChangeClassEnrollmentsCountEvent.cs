using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeClassEnrollmentsCountEvent: Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public bool IsIncrease { get; set; }
}
