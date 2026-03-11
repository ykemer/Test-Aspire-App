namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public record UpdateStudentEnrollmentsCountCommand(Guid StudentId, bool IsIncrease) : IRequest<ErrorOr<Updated>>;
