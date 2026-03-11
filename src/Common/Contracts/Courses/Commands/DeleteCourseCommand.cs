namespace Contracts.Courses.Commands;

public class DeleteCourseCommand
{
  public Guid CourseId { get; set; }
  public string UserId { get; init; }
}
