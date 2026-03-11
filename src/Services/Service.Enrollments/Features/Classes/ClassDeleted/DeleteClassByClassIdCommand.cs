namespace Service.Enrollments.Features.Classes.ClassDeleted;

public record DeleteClassByClassIdCommand(Guid CourseId, Guid ClassId) : IRequest<ErrorOr<Deleted>>;
