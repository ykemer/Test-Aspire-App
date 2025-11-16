namespace Service.Enrollments.Features.Classes.ClassDeleted;

public record DeleteClassByClassIdCommand(string CourseId, string ClassId) : IRequest<ErrorOr<Deleted>>;
