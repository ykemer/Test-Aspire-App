using Contracts.Enrollments.Events;

namespace Service.Enrollments.AsyncDataServices;

public interface IMessageBusClient
{
  void PublishStudentEnrolledEvent(StudentEnrolledEvent studentEnrolledEvent);
  void PublishStudentUnenrolledEvent(StudentUnenrolledEvent studentUnenrolledEvent);
}
