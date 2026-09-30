using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public record GetCourseQuery(Guid Id, IReadOnlyCollection<Guid> EnrolledClasses, bool ShowAll)
  : IRequest<ErrorOr<Course>>;
