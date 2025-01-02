namespace Contracts.Courses.Responses;

public class CourseListItemResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }

    public bool IsUserEnrolled { get; init; } = false;

    public int EnrollmentsCount { get; init; } = 0;
}