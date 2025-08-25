namespace Service.Enrollments.Features.Classes.DeleteEnrollmentsByClass;

public record DeleteEnrollmentsByClassCommand(string CourseId, string ClassId) : IRequest<ErrorOr<Deleted>>;
