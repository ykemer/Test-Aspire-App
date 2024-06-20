namespace Aspire_App.Web.Contracts.Requests.Courses;

public class ChangeEnrollmentForTheCourseByAdmin: ChangeEnrollmentForTheCourse
{
    public Guid StudentId { get; set; }
   
}