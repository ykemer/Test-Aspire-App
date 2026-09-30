using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.ClassDeleted;

/// <summary>
/// Removes the copy of a class that was deleted in the Courses service.
/// A class that is already gone counts as success, so a repeated message does no harm.
/// </summary>
public class DeleteClassByClassIdCommandHandler : IRequestHandler<DeleteClassByClassIdCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteClassByClassIdCommandHandler> _logger;

  public DeleteClassByClassIdCommandHandler(ApplicationDbContext dbContext,
    ILogger<DeleteClassByClassIdCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteClassByClassIdCommand request,
    CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == request.ClassId, cancellationToken);
    if (courseClass is null)
    {
      _logger.LogInformation("Class {ClassId} is already gone, nothing to delete", request.ClassId);
      return Result.Deleted;
    }

    var hasEnrollments = await _dbContext.Enrollments.AnyAsync(e => e.ClassId == request.ClassId, cancellationToken);
    if (hasEnrollments)
    {
      // Courses only deletes classes without students, so this means the two services disagree.
      _logger.LogError("Class {ClassId} was deleted in Courses but still has enrollments here", request.ClassId);
      return ClassErrors.HasEnrollments(request.ClassId);
    }

    _dbContext.Classes.Remove(courseClass);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Class {ClassId} of course {CourseId} was deleted", request.ClassId, request.CourseId);
    return Result.Deleted;
  }
}
