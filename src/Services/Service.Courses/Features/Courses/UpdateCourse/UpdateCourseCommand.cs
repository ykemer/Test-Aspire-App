namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommand : IRequest<ErrorOr<Updated>>
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }
}
