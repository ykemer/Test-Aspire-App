namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public record UpdateStudentEnrollmentsCountCommand(string StudentId, bool IsIncrease) : IRequest<ErrorOr<Updated>>;
