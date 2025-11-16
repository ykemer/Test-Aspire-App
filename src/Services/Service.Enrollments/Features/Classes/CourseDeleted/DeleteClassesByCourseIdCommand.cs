namespace Service.Enrollments.Features.Classes.CourseDeleted;

public record DeleteClassesByCourseIdCommand(string CourseId) : IRequest<ErrorOr<Deleted>>;
