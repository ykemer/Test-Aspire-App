using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public class GetCourseEnrollmentsRequestHandler : IRequestHandler<GetCourseEnrollmentsRequest, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public GetCourseEnrollmentsRequestHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async ValueTask<ErrorOr<List<Enrollment>>> Handle(GetCourseEnrollmentsRequest request,
    CancellationToken cancellationToken)
  {
    var enrollments = await _dbContext.Enrollments
      .AsNoTracking()
      .Where(e => e.CourseId == request.CourseId)
      .ToListAsync(cancellationToken);

    return enrollments;
  }
}
