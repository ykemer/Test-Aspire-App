using Library.Database;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

/// <summary>
/// Creates a new course. Two courses cannot share a name (upper/lower case is ignored).
/// </summary>
public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ErrorOr<Course>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateCourseCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public CreateCourseCommandHandler(ApplicationDbContext dbContext, ILogger<CreateCourseCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Course>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
  {
    var lowerCaseName = request.Name.ToLowerInvariant();
    var nameIsTaken = await _dbContext.Courses
      .AnyAsync(course => course.Name.ToLower() == lowerCaseName, cancellationToken);
    if (nameIsTaken)
    {
      _logger.LogWarning("Course with name {CourseName} already exists", request.Name);
      return CourseErrors.NameAlreadyTaken(request.Name);
    }

    var course = request.ToCourse(_timeProvider.GetUtcNow().UtcDateTime);
    _dbContext.Courses.Add(course);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation())
    {
      // Another request created the same name between our check and our save. The unique index caught it.
      _logger.LogWarning("Course with name {CourseName} was created concurrently", request.Name);
      return CourseErrors.NameAlreadyTaken(request.Name);
    }

    _logger.LogInformation("Course {CourseId} was created", course.Id);
    return course;
  }
}
