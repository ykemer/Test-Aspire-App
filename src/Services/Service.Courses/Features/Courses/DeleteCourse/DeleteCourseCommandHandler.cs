using Service.Courses.Common.Database;

namespace Service.Courses.Features.Courses.DeleteCourse;

/// <summary>
/// Deletes a course and all its classes. A course with enrolled students cannot be deleted.
/// Publishing the "course deleted" event is the job of <see cref="DeleteCourseCommandConsumer"/>.
/// </summary>
public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteCourseCommandHandler> _logger;

  public DeleteCourseCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteCourseCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
  {
    var course = await _dbContext.Courses
      .Include(course => course.CourseClasses)
      .FirstOrDefaultAsync(course => course.Id == request.Id, cancellationToken);
    if (course is null)
    {
      _logger.LogWarning("Cannot delete course {CourseId} because it was not found", request.Id);
      return CourseErrors.NotFound(request.Id);
    }

    if (course.TotalStudents > 0)
    {
      _logger.LogWarning("Cannot delete course {CourseId} because it has enrolled students", request.Id);
      return CourseErrors.HasEnrolledStudents(request.Id);
    }

    _dbContext.Classes.RemoveRange(course.CourseClasses);
    _dbContext.Courses.Remove(course);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      // For example, a student enrolled while we were deleting. Nothing is deleted in that case.
      _logger.LogWarning("Course {CourseId} was modified while being deleted", request.Id);
      return CourseErrors.ModifiedConcurrently(request.Id);
    }

    _logger.LogInformation("Course {CourseId} was deleted", request.Id);
    return Result.Deleted;
  }
}
