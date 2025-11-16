namespace Contracts.Courses.Commands;

public class CreateCourseCommand
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string UserId { get; init; }
}
