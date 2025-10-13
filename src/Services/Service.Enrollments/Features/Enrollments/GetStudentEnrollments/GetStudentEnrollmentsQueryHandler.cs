using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public class GetStudentEnrollmentsQueryHandler : IRequestHandler<GetStudentEnrollmentsQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetStudentEnrollmentsQueryHandler> _logger;

  public GetStudentEnrollmentsQueryHandler(ApplicationDbContext dbContext,
    ILogger<GetStudentEnrollmentsQueryHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<List<Enrollment>>> Handle(GetStudentEnrollmentsQuery request,
    CancellationToken cancellationToken)
  {
    var enrollments = await _dbContext.Enrollments
      .AsNoTracking()
      .Where(e =>
        e.StudentId == request.StudentId &&
        (string.IsNullOrEmpty(request.CourseId) || e.CourseId == request.CourseId)
      )
      .ToListAsync(cancellationToken);

    return enrollments;
  }
}
