namespace Service.Courses.Features.Courses.DeleteCourse;

public record DeleteCourseCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
