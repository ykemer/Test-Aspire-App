namespace Service.Courses.Features.Courses.UpdateCourse;

public record UpdateCourseCommand : IRequest<ErrorOr<Updated>>
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string Description { get; init; }
}
