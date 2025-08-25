using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public record CreateCourseCommand(string Name, string Description) : IRequest<ErrorOr<Course>>;
