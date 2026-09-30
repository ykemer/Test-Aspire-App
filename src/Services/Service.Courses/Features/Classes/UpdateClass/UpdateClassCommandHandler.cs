using Service.Courses.Common.Database;

namespace Service.Courses.Features.Classes.UpdateClass;

/// <summary>
/// Changes the dates and size of a class.
/// The new maximum cannot be lower than the number of students already enrolled.
/// </summary>
public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UpdateClassCommandHandler(ApplicationDbContext dbContext, ILogger<UpdateClassCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes.FirstOrDefaultAsync(
      courseClass => courseClass.Id == request.Id && courseClass.CourseId == request.CourseId, cancellationToken);
    if (courseClass is null)
    {
      _logger.LogWarning("Cannot update class {ClassId} of course {CourseId} because it was not found",
        request.Id, request.CourseId);
      return ClassErrors.NotFound(request.Id, request.CourseId);
    }

    if (courseClass.TotalStudents > request.MaxStudents)
    {
      _logger.LogWarning("Cannot update class {ClassId}: new maximum is below the enrolled count", request.Id);
      return ClassErrors.MaxStudentsBelowEnrolledCount(request.Id);
    }

    courseClass.ApplyUpdate(request, _timeProvider.GetUtcNow().UtcDateTime);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      // For example, a student enrolled after we read the class. The enrolled count may now be too high.
      _logger.LogWarning("Class {ClassId} was modified concurrently", request.Id);
      return ClassErrors.ModifiedConcurrently(request.Id);
    }

    _logger.LogInformation("Class {ClassId} was updated", request.Id);
    return Result.Updated;
  }
}
