using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ErrorOr<Course>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateCourseCommandHandler> _logger;

  public CreateCourseCommandHandler(ApplicationDbContext dbContext, ILogger<CreateCourseCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Course>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
  {
    var existingCourse =
      await _dbContext.Courses.FirstOrDefaultAsync(course => course.Name.ToLower() == request.Name.ToLower(),
        cancellationToken);
    if (existingCourse != null)
    {
      _logger.LogWarning("Course with name {CourseName} already exists", request.Name);
      return Error.Conflict("courses_service.create_course.already_exists", $"Course {request.Name} already exists");
    }

    var course = request.MapToCourse();

    await _dbContext.Courses.AddAsync(course, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogTrace("Course {CourseName} is being created", request.Name);
    return course;
  }
}
