using Contracts.AsyncMessages;

namespace Contracts.Courses.Events;

public class DecreaseCourseEnrollments: AsyncMessage
{
  public string CourseId { get; set; }
}
