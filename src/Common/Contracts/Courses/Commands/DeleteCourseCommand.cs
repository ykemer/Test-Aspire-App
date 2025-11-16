namespace Contracts.Courses.Commands;

public class DeleteCourseCommand
{
  public string CourseId { get; set; }
  public string UserId { get; init; }
}
