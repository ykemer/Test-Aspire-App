using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;

public class
  ListOfEnrollmentsByCoursesHandler : IRequestHandler<ListOfEnrollmentsByCoursesQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListOfEnrollmentsByCoursesHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async Task<ErrorOr<List<Enrollment>>> Handle(ListOfEnrollmentsByCoursesQuery request,
    CancellationToken cancellationToken)
  {
    var query = _dbContext.Enrollments
      .Where(i => request.CourseIds.Contains(i.CourseId));

    if (!string.IsNullOrEmpty(request.StudentId))
    {
      query = query.Where(i => i.StudentId == request.StudentId);
    }


    return await query.ToListAsync(cancellationToken);
  }
}
