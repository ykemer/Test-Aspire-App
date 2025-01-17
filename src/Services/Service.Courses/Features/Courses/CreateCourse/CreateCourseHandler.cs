using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.CreateCourse;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, ErrorOr<Course>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateCourseHandler> _logger;

  public CreateCourseHandler(ApplicationDbContext dbContext, ILogger<CreateCourseHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Course>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
  {
    var existingCourse =
      await _dbContext.Courses.FirstOrDefaultAsync(course => course.Name.ToLower() == request.Name.ToLower(),
        cancellationToken);
    if (existingCourse != null)
    {
      _logger.LogWarning("Course with name {CourseName} already exists", request.Name);
      return Error.Conflict("courses_service.create_course.already_exists", $"Course {request.Name} already exists");
    }

    Course? course = new() { Description = request.Description, Name = request.Name, EnrollmentsCount = 0 };

    await _dbContext.Courses.AddAsync(course, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogTrace("Course {CourseName} is being created", request.Name);
    return course;
  }
}
