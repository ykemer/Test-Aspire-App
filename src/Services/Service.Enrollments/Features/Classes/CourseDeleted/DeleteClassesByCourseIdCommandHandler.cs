using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.CourseDeleted;

public class
  DeleteClassesByCourseIdCommandHandler : IRequestHandler<DeleteClassesByCourseIdCommand, ErrorOr<Deleted>>
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
    var existingClasses = _dbContext.Classes.Where(course => course.CourseId == request.CourseId).ToList();
    if (existingClasses.Count == 0)
    {
      _logger.LogInformation("No classes were found for course: {CourseId}", request.CourseId);
      return Result.Deleted;
    }

    var existingEnrollments =
      await _dbContext.Enrollments.CountAsync(enrollment => enrollment.CourseId == request.CourseId, cancellationToken);

    if (existingEnrollments > 0)
    {
      _logger.LogError("Classes for course with ID {CourseId} can not be deleted because of existing subscriptions",
        request.CourseId);
      return Error.Conflict(
        description: $"Classes for course with ID {request.CourseId} can not be deleted because of existing subscriptions");
    }

    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.CourseId == request.CourseId));
    _dbContext.RemoveRange(_dbContext.Classes.Where(course => course.CourseId == request.CourseId));

    await _dbContext.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Deleting enrollments for course {CourseId}", request.CourseId);
    return Result.Deleted;
  }
}
