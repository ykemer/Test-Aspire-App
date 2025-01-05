using Contracts.Courses.Events;

namespace Service.Courses.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishCourseDeletedMessage(CourseDeletedEvent courseDeletedEvent);
}