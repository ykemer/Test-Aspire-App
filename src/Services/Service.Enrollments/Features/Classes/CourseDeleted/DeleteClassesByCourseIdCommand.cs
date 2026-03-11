namespace Service.Enrollments.Features.Classes.CourseDeleted;

public record DeleteClassesByCourseIdCommand(Guid CourseId) : IRequest<ErrorOr<Deleted>>;
