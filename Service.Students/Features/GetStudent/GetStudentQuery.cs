using Service.Students.Entitites;

namespace Service.Students.Features.GetStudent;

public record GetStudentQuery(string StudentId) : IRequest<ErrorOr<Student>>;