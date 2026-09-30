using Service.Students.Common.Database;

namespace Service.Students.Features.DeleteStudent;

/// <summary>
/// Deletes a student who is not enrolled in any class.
/// The check and the delete happen in one SQL statement, so a student who enrolls at the same moment
/// is never deleted by mistake. Publishing the "student deleted" event is the job of <see cref="StudentsService"/>.
/// </summary>
public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteStudentCommandHandler> _logger;

  public DeleteStudentCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteStudentCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
  {
    var deletedRows = await _dbContext.Students
      .Where(student => student.Id == request.StudentId && student.EnrollmentsCount == 0)
      .ExecuteDeleteAsync(cancellationToken);

    if (deletedRows == 1)
    {
      _logger.LogInformation("Student {StudentId} was deleted", request.StudentId);
      return Result.Deleted;
    }

    // Nothing was deleted. Find out why.
    var studentExists = await _dbContext.Students.AnyAsync(s => s.Id == request.StudentId, cancellationToken);
    if (!studentExists)
    {
      _logger.LogWarning("Cannot delete student {StudentId} because it was not found", request.StudentId);
      return StudentErrors.NotFound(request.StudentId);
    }

    _logger.LogWarning("Cannot delete student {StudentId} because they are enrolled in classes", request.StudentId);
    return StudentErrors.HasEnrollments(request.StudentId);
  }
}
