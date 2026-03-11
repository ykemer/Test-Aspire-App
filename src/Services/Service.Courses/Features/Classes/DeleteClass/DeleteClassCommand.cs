namespace Service.Courses.Features.Classes.DeleteClass;

public record DeleteClassCommand(Guid Id, Guid CourseId) : IRequest<ErrorOr<Deleted>>;
