using Contracts.AsyncMessages;

namespace Contracts.Students.Events;

public class DecreaseStudentEnrollmentsEvent : AsyncMessage
{
  public string StudentId { get; init; }
}
