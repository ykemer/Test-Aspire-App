namespace Contracts.Courses.Commands;

public class UpdateCourseCommand
{
  public string CourseId { get; init; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string UserId { get; init; }
}
