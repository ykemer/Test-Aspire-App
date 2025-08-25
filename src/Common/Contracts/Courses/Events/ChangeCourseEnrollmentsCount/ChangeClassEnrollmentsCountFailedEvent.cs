using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeClassEnrollmentsCountFailedEvent: Event
{

  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public string ErrorMessage { get; set; }
}
