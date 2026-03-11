using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public record GetCourseQuery(Guid Id, List<Guid> EnrolledClasses, bool ShowAll) : IRequest<ErrorOr<Course>>;
