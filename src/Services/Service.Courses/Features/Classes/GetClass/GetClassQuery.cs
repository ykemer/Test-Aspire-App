using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.GetClass;

public record GetClassQuery(Guid Id, Guid CourseId, List<Guid> EnrolledClasses, bool ShowAll)
  : IRequest<ErrorOr<Class>>;
