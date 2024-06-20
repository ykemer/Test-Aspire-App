using Aspire_App.ApiService.Application.Students.Responses;

namespace Aspire_App.Web.Models.Courses;

public class CourseResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }

    public bool Enrolled { get; init; } = false;
    public List<StudentResponse>? EnrolledStudents { get; init; }
}