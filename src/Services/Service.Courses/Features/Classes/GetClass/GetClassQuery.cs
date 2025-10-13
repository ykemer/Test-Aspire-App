using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.GetClass;

public record GetClassQuery(string Id, string CourseId, List<string> EnrolledClasses, bool ShowAll) : IRequest<ErrorOr<Class>>;
