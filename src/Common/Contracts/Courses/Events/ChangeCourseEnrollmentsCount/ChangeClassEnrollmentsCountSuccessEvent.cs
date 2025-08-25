using Contracts.Common;

namespace Contracts.Courses.Events.ChangeCourseEnrollmentsCount;

public class ChangeClassEnrollmentsCountSuccessEvent: Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
