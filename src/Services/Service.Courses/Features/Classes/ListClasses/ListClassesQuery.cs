using Contracts.Common;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.ListClasses;

public sealed class ListClassesQuery : PagedQuery, IRequest<ErrorOr<PagedList<Class>>>
{
  public required Guid CourseId { get; init; }
  public IReadOnlyCollection<Guid> EnrolledClasses { get; init; } = [];
  public bool ShowAll { get; init; }
}
