using Contracts.Common;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.ListClasses;

public sealed class ListClassesQuery : PagedQuery, IRequest<ErrorOr<PagedList<Class>>>
{
  public required string CourseId { get; init; }
  public List<string> EnrolledClasses { get; set; } = new();
  public bool ShowAll { get; set; }
}

