using Contracts.Enrollments.Events;
using Service.Enrollments.Entities;

namespace Service.Enrollments.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishStudentEnrolledEvent(StudentEnrolledEvent studentEnrolledEvent);
    void PublishStudentUnenrolledEvent(StudentUnenrolledEvent studentUnenrolledEvent);
}