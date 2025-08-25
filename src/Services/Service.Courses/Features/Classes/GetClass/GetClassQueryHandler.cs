using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Classes.GetClass;

public class GetClassQueryHandler : IRequestHandler<GetClassQuery, ErrorOr<Class>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetClassQueryHandler> _logger;

  public GetClassQueryHandler(ILogger<GetClassQueryHandler> logger, ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Class>> Handle(GetClassQuery request, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes
      .AsNoTracking()
      .FirstOrDefaultAsync(courseClass =>
          courseClass.Id == request.Id &&
          courseClass.CourseId == request.CourseId &&
          (request.ShowAll ||
           ((courseClass.RegistrationDeadline > DateTime.UtcNow && courseClass.TotalStudents < courseClass.MaxStudents) ||
            request.EnrolledClasses.Contains(courseClass.Id))
          ), cancellationToken
      );

    if (courseClass != null)
    {
      return courseClass;
    }

    _logger.Log(LogLevel.Warning, "Class with id {RequestId} for course {CourseId} was not found", request.Id,
      request.CourseId);
    return Error.NotFound("courses_service.get_class.not_found",
      $"Class with id {request.Id} for course {request.CourseId} was not found");
  }
}
