namespace Contracts.Courses.Requests;

public class ChangeCourseEnrollmentAdminRequest : ChangeCourseEnrollmentRequest
{
    public Guid StudentId { get; set; }
}