namespace Contracts.Courses.Requests.Enrollments;

public class ChangeCourseEnrollmentRequest
{

  public Guid StudentId { get; set; } = Guid.Empty;
}
