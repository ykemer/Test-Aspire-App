using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ErrorOr<Class>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateClassCommandHandler> _logger;

  public CreateClassCommandHandler(ApplicationDbContext dbContext, ILogger<CreateClassCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Class>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
  {
    var existingCourse =
      await _dbContext.Courses.FirstOrDefaultAsync(course => course.Id == request.CourseId,
        cancellationToken);

    if (existingCourse == null)
    {
      _logger.LogWarning("Class with ID {ClassId} does not exists", request.CourseId);
      return Error.Conflict("courses_service.create_class.course_does_not_exist",
        $"Course {request.CourseId} already exists");
    }

    var courseClass = request.MapToClass();
    await _dbContext.Classes.AddAsync(courseClass, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogTrace("Course Class {StartDate} - {EndDate} is being created",
      request.CourseStartDate.ToString("yyyy-MM-dd"),
      request.CourseEndDate.ToString("yyyy-MM-dd"));
    return courseClass;
  }
}
