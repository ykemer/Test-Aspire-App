using Contracts.Students.Responses;

namespace Aspire_App.ApiService.Application.Courses.Responses;

public class CourseWithEnrolledStudentsResponse : CourseResponse
{
    public List<StudentResponse> EnrolledStudents { get; set; } = new();
}