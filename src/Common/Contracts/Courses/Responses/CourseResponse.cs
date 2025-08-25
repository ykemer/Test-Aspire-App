namespace Contracts.Courses.Responses;

public class CourseResponse
{
  public Guid Id { get; init; }

  public string Name { get; init; }
  public string Description { get; init; }

  public int TotalStudents { get; init; } = 0;
}
