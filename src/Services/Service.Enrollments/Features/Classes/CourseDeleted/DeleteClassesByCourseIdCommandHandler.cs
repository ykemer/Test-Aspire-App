using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.CourseDeleted;

/// <summary>
/// Removes the copies of all classes of a course that was deleted in the Courses service.
/// A course without classes here counts as success, so a repeated message does no harm.
/// </summary>
public class DeleteClassesByCourseIdCommandHandler
  : IRequestHandler<DeleteClassesByCourseIdCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteClassesByCourseIdCommandHandler> _logger;

  public DeleteClassesByCourseIdCommandHandler(ApplicationDbContext dbContext,
    ILogger<DeleteClassesByCourseIdCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteClassesByCourseIdCommand request,
    CancellationToken cancellationToken)
  {
    var classes = await _dbContext.Classes
      .Where(courseClass => courseClass.CourseId == request.CourseId)
      .ToListAsync(cancellationToken);
    if (classes.Count == 0)
    {
      _logger.LogInformation("Course {CourseId} has no classes here, nothing to delete", request.CourseId);
      return Result.Deleted;
    }

    var hasEnrollments = await _dbContext.Enrollments.AnyAsync(e => e.CourseId == request.CourseId, cancellationToken);
    if (hasEnrollments)
    {
      // Courses only deletes courses without students, so this means the two services disagree.
      _logger.LogError("Course {CourseId} was deleted in Courses but still has enrollments here", request.CourseId);
      return ClassErrors.CourseHasEnrollments(request.CourseId);
    }

    _dbContext.Classes.RemoveRange(classes);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("{ClassCount} classes of course {CourseId} were deleted", classes.Count, request.CourseId);
    return Result.Deleted;
  }
}
