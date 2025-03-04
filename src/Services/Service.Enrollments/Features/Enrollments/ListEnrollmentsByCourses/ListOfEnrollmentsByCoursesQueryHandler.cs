﻿using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;

public class
  ListOfEnrollmentsByCoursesQueryHandler : IRequestHandler<ListOfEnrollmentsByCoursesQuery, ErrorOr<List<Enrollment>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListOfEnrollmentsByCoursesQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async Task<ErrorOr<List<Enrollment>>> Handle(ListOfEnrollmentsByCoursesQuery request,
    CancellationToken cancellationToken)
  {
    var query = _dbContext.Enrollments
      .AsNoTracking()
      .Where(i => request.CourseIds.Contains(i.CourseId));

    if (!string.IsNullOrEmpty(request.StudentId))
    {
      query = query.Where(i => i.StudentId == request.StudentId);
    }


    return await query.ToListAsync(cancellationToken);
  }
}
