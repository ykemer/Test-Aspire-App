using Aspire_App.ApiService.Application.Students.Responses;

namespace Aspire_App.Web.Models.Courses;

public class CoursesListItem
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }
    public List<StudentResponse>? EnrolledStudents { get; init; }
}