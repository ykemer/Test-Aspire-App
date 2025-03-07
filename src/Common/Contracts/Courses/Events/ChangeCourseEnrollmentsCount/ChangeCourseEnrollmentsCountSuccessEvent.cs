using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeCourseEnrollmentsCountSuccessEvent: Event
{
  public string CourseId { get; set; }
}
