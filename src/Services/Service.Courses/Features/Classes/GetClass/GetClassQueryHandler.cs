using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.GetClass;

/// <summary>
/// Returns one class. Unless "show all" is set, the class must be visible (see <see cref="ClassVisibility"/>).
/// </summary>
public class GetClassQueryHandler : IRequestHandler<GetClassQuery, ErrorOr<Class>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetClassQueryHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public GetClassQueryHandler(ILogger<GetClassQueryHandler> logger, ApplicationDbContext dbContext,
    TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Class>> Handle(GetClassQuery request, CancellationToken cancellationToken)
  {
    var classes = _dbContext.Classes
      .AsNoTracking()
      .Where(courseClass => courseClass.Id == request.Id && courseClass.CourseId == request.CourseId);

    if (!request.ShowAll)
    {
      var now = _timeProvider.GetUtcNow().UtcDateTime;
      classes = classes.OnlyVisibleClasses(request.EnrolledClasses, now);
    }

    var foundClass = await classes.FirstOrDefaultAsync(cancellationToken);
    if (foundClass is null)
    {
      _logger.LogWarning("Class {ClassId} of course {CourseId} was not found", request.Id, request.CourseId);
      return ClassErrors.NotFound(request.Id, request.CourseId);
    }

    return foundClass;
  }
}
