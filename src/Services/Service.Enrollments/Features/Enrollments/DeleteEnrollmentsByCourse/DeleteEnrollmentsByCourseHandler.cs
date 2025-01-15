namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

public class DeleteEnrollmentsByCourseHandler : IRequestHandler<DeleteEnrollmentsByCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByCourseHandler> _logger;

  public DeleteEnrollmentsByCourseHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByCourseHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByCourseCommand request,
    CancellationToken cancellationToken)
  {
    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.CourseId == request.CourseId));
    await _dbContext.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Deleting enrollments for course {CourseId}", request.CourseId);
    // TODO: Notify students service about the deletion
    return Result.Deleted;
  }
}
