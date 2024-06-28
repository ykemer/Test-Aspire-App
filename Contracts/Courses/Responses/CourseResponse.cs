using Contracts.Students.Responses;

namespace Contracts.Courses.Responses;

public class CourseResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }

    public bool Enrolled { get; init; } = false;
    public List<StudentResponse>? EnrolledStudents { get; init; }
}