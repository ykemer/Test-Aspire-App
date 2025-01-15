namespace Contracts.Courses.Requests;

public class ChangeCourseEnrollmentRequest
{
  public Guid CourseId { get; set; }
  public Guid StudentId { get; set; } = Guid.Empty;
}
