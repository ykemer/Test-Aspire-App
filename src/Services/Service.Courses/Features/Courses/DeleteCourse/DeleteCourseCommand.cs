namespace Service.Courses.Features.Courses.DeleteCourse;

public record DeleteCourseCommand(string Id) : IRequest<ErrorOr<Deleted>>;
