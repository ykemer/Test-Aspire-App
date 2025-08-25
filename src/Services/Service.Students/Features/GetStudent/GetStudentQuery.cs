using Service.Students.Database.Entities;

namespace Service.Students.Features.GetStudent;

public record GetStudentQuery(string StudentId) : IRequest<ErrorOr<Student>>;
