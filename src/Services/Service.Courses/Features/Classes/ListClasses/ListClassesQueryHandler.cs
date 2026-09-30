using Contracts.Common;

using Library.Database;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.ListClasses;

/// <summary>
/// Returns one page of the classes of a course, newest first.
/// Unless "show all" is set, only visible classes are returned (see <see cref="ClassVisibility"/>).
/// </summary>
public class ListClassesQueryHandler : IRequestHandler<ListClassesQuery, ErrorOr<PagedList<Class>>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly TimeProvider _timeProvider;

  public ListClassesQueryHandler(ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<PagedList<Class>>> Handle(ListClassesQuery request,
    CancellationToken cancellationToken)
  {
    var classes = _dbContext.Classes
      .AsNoTracking()
      .Where(courseClass => courseClass.CourseId == request.CourseId);

    if (!request.ShowAll)
    {
      var now = _timeProvider.GetUtcNow().UtcDateTime;
      classes = classes.OnlyVisibleClasses(request.EnrolledClasses, now);
    }

    return await classes
      .OrderByDescending(courseClass => courseClass.CreatedAt)
      .ToPagedListAsync(request.PageNumber, request.PageSize, cancellationToken);
  }
}
