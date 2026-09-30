using Service.Courses.Common.Database;

namespace Service.Courses.Features.Classes.DeleteClass;

/// <summary>
/// Deletes a class. A class with enrolled students cannot be deleted.
/// Publishing the "class deleted" event is the job of <see cref="DeleteClassCommandConsumer"/>.
/// </summary>
public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteClassCommandHandler> _logger;

  public DeleteClassCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteClassCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes.FirstOrDefaultAsync(
      courseClass => courseClass.Id == request.Id && courseClass.CourseId == request.CourseId, cancellationToken);
    if (courseClass is null)
    {
      _logger.LogWarning("Cannot delete class {ClassId} of course {CourseId} because it was not found",
        request.Id, request.CourseId);
      return ClassErrors.NotFound(request.Id, request.CourseId);
    }

    if (courseClass.TotalStudents > 0)
    {
      _logger.LogWarning("Cannot delete class {ClassId} because it has enrolled students", request.Id);
      return ClassErrors.HasEnrolledStudents(request.Id);
    }

    _dbContext.Classes.Remove(courseClass);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      // For example, a student enrolled while we were deleting. Nothing is deleted in that case.
      _logger.LogWarning("Class {ClassId} was modified while being deleted", request.Id);
      return ClassErrors.ModifiedConcurrently(request.Id);
    }

    _logger.LogInformation("Class {ClassId} was deleted", request.Id);
    return Result.Deleted;
  }
}
