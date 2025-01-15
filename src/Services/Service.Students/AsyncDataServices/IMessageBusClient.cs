using Contracts.Students.Events;

namespace Service.Students.AsyncDataServices;

public interface IMessageBusClient
{
  void PublishStudentDeletedEvent(StudentDeletedEvent studentDeletedEvent);
}
