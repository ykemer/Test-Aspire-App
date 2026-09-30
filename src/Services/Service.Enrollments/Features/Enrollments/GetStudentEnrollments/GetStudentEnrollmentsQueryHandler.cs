using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

/// <summary>
/// Returns all enrollments of a student, optionally only for one course.
/// </summary>
public class GetStudentEnrollmentsQueryHandler : IRequestHandler<GetStudentEnrollmentsQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public GetStudentEnrollmentsQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async ValueTask<ErrorOr<List<Enrollment>>> Handle(GetStudentEnrollmentsQuery request,
    CancellationToken cancellationToken)
  {
    var enrollments = _dbContext.Enrollments
      .AsNoTracking()
      .Where(enrollment => enrollment.StudentId == request.StudentId);

    if (request.CourseId is not null)
    {
      enrollments = enrollments.Where(enrollment => enrollment.CourseId == request.CourseId);
    }

    return await enrollments.ToListAsync(cancellationToken);
  }
}
