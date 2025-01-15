namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByStudent;

public class DeleteEnrollmentsByStudentHandler : IRequestHandler<DeleteEnrollmentsByStudentCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByStudentHandler> _logger;

  public DeleteEnrollmentsByStudentHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByStudentHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByStudentCommand request,
    CancellationToken cancellationToken)
  {
    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.StudentId == request.StudentId));
    await _dbContext.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Deleting enrollments for Student {StudentId}", request.StudentId);
    // TODO: Notify courses service about the deletion
    return Result.Deleted;
  }
}
