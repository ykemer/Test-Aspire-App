using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public record GetCourseQuery(string Id, List<string> EnrolledClasses, bool ShowAll) : IRequest<ErrorOr<Course>>;
