namespace Aspire_App.ApiService.Application.Courses.Responses;

public class CourseResponse
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enrolled { get; set; } = false;
}