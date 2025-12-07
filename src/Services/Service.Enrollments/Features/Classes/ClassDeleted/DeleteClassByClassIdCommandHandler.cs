using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.ClassDeleted;

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
    var existingClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == request.ClassId, cancellationToken);

    if (existingClass is null)
    {
      _logger.LogError("Class with ID {ClassId} not found for deletion", request.ClassId);
      return Error.NotFound(description: $"Class with ID {request.ClassId} not found");
    }

    var existingEnrollments = await _dbContext.Enrollments.CountAsync(enrollment =>
      enrollment.CourseId == request.CourseId && enrollment.ClassId == request.ClassId, cancellationToken);

    if (existingEnrollments > 0)
    {
      _logger.LogError("Class with ID {ClassId} can not be deleted because of existing subscriptions", request.ClassId);
      return Error.Conflict(
        description: "Class with ID {ClassId} can not be deleted because of existing subscriptions");
    }

    _dbContext.Classes.Remove(existingClass);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Deleting class  {ClassId} for course {CourseId}", request.ClassId,
      request.CourseId);

    return Result.Deleted;
  }
}
