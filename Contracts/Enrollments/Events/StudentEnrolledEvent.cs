using Contracts.AsyncMessages;

namespace Contracts.Enrollments.Events;

public class StudentEnrolledEvent: AsyncMessage
{
    public string CourseId { get; set; }
    public string StudentId { get; set; }
}