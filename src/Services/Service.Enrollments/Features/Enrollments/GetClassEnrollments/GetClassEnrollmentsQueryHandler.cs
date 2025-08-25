using Service.Enrollments.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public class GetClassEnrollmentsQueryHandler : IRequestHandler<GetClassEnrollmentsQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public GetClassEnrollmentsQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async ValueTask<ErrorOr<List<Enrollment>>> Handle(GetClassEnrollmentsQuery query,
    CancellationToken cancellationToken)
  {
    var enrollments = await _dbContext.Enrollments
      .AsNoTracking()
      .Where(e => e.CourseId == query.CourseId && e.ClassId == query.ClassId)
      .ToListAsync(cancellationToken);

    return enrollments;
  }
}
