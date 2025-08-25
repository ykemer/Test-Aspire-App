namespace Service.Courses.Features.Classes.DeleteClass;

public record DeleteClassCommand(string Id, string CourseId) : IRequest<ErrorOr<Deleted>>;
