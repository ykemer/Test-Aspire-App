namespace Contracts.Courses.Requests;

public class ChangeCourseEnrollmentAdminRequest : ChangeCourseEnrollmentRequest
{
  public string StudentId { get; set; }
}
