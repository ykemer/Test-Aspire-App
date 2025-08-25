using Service.Enrollments.Database.Entities;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public class GetCourseEnrollmentsQueryHandler : IRequestHandler<GetCourseEnrollmentsQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public GetCourseEnrollmentsQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async ValueTask<ErrorOr<List<Enrollment>>> Handle(GetCourseEnrollmentsQuery query,
    CancellationToken cancellationToken)
  {
    var enrollments = await _dbContext.Enrollments
      .AsNoTracking()
      .Where(e => e.CourseId == query.CourseId)
      .ToListAsync(cancellationToken);

    return enrollments;
  }
}
