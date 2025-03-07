using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeCourseEnrollmentsCountFailedEvent: Event
{

  public string CourseId { get; set; }
  public string ErrorMessage { get; set; }
}
