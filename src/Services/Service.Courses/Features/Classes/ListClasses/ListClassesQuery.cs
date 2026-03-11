using Contracts.Common;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.ListClasses;

public sealed class ListClassesQuery : PagedQuery, IRequest<ErrorOr<PagedList<Class>>>
{
  public required Guid CourseId { get; init; }
  public List<Guid> EnrolledClasses { get; set; } = new();
  public bool ShowAll { get; set; }
}
