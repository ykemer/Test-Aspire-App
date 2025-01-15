using Contracts.AsyncMessages;

namespace Contracts.Students.Events;

public class StudentDeletedEvent : AsyncMessage
{
  public string StudentId { get; set; }
}
