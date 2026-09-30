using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;

namespace Service.Courses.Features.Classes.CreateClass;

/// <summary>
/// Adds a new class to an existing course.
/// </summary>
public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ErrorOr<Class>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public CreateClassCommandHandler(ApplicationDbContext dbContext, ILogger<CreateClassCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Class>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
  {
    var courseExists = await _dbContext.Courses.AnyAsync(course => course.Id == request.CourseId, cancellationToken);
    if (!courseExists)
    {
      _logger.LogWarning("Cannot create class because course {CourseId} was not found", request.CourseId);
      return CourseErrors.NotFound(request.CourseId);
    }

    var courseClass = request.ToClass(_timeProvider.GetUtcNow().UtcDateTime);
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Class {ClassId} was created for course {CourseId}", courseClass.Id, courseClass.CourseId);
    return courseClass;
  }
}
