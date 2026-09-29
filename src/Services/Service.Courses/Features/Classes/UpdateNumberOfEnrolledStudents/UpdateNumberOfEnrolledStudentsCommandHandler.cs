using Service.Courses.Common.Database;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public class
  UpdateNumberOfEnrolledStudentsCommandHandler : IRequestHandler<UpdateNumberOfEnrolledStudentsCommand,
  ErrorOr<Updated>>
{
  private const int MaxConcurrencyAttempts = 3;

  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> _logger;

  public UpdateNumberOfEnrolledStudentsCommandHandler(ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> logger,
    ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateNumberOfEnrolledStudentsCommand request,
    CancellationToken cancellationToken)
  {
    for (var attempt = 0; attempt < MaxConcurrencyAttempts; attempt++)
    {
      var existingCourse = await _dbContext.Courses
        .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
      if (existingCourse == null)
      {
        _logger.LogWarning("Course {CourseId} not found", request.CourseId);
        return Error.NotFound("course_service.increase_number_of_enrolled_students.course.not_found",
          $"Course {request.CourseId} not found");
      }

      var existingClass = await _dbContext.Classes
        .FirstOrDefaultAsync(c => c.Id == request.ClassId && c.CourseId == request.CourseId, cancellationToken);
      if (existingClass == null)
      {
        _logger.LogWarning("Class {ClassId} for course {CourseId} not found", request.ClassId, request.CourseId);
        return Error.NotFound("course_service.increase_number_of_enrolled_students.class.not_found",
          $"Class {request.ClassId} for course {request.CourseId} not found");
      }

      existingCourse.TotalStudents += request.IsIncrease ? 1 : -1;
      if (existingCourse.TotalStudents < 0)
      {
        _logger.LogError("Course {CourseId} enrollments count cannot be negative", request.CourseId);
        return Error.Conflict("course_service.update_course_enrollments_count.invalid_enrollments_count",
          "Course enrollments count cannot be negative");
      }

      existingClass.TotalStudents += request.IsIncrease ? 1 : -1;
      if (existingClass.TotalStudents < 0)
      {
        _logger.LogError("Class {ClassId} for course {CourseId} enrollments count cannot be negative",
          request.ClassId, request.CourseId);
        return Error.Conflict("course_service.update_class_enrollments_count.invalid_enrollments_count",
          "Class enrollments count cannot be negative");
      }

      try
      {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Updated;
      }
      catch (DbUpdateConcurrencyException)
      {
        _logger.LogInformation(
          "Concurrent enrollment count change detected for course {CourseId}/class {ClassId}, retrying (attempt {Attempt})",
          request.CourseId, request.ClassId, attempt + 1);
        _dbContext.ChangeTracker.Clear();
      }
    }

    _logger.LogWarning("Too many concurrent enrollment count updates for course {CourseId}/class {ClassId}",
      request.CourseId, request.ClassId);
    return Error.Conflict("course_service.update_class_enrollments_count.concurrency_conflict",
      "Too many concurrent enrollment count updates, please retry.");
  }
}
