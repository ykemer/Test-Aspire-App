namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommand: IRequest<ErrorOr<Updated>>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}