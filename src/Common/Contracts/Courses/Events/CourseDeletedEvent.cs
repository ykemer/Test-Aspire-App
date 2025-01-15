using Contracts.AsyncMessages;

namespace Contracts.Courses.Events;

public class CourseDeletedEvent : AsyncMessage
{
  public string CourseId { get; set; }
}
