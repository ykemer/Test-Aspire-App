using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public record GetCourseQuery(string Id) : IRequest<ErrorOr<Course>>;
