using Contracts.Common;

using Library.Database;

using Service.Students.Common.Database;
using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.ListStudents;

/// <summary>
/// Returns one page of students, oldest first (ids are time-ordered, so ordering by id keeps pages stable).
/// </summary>
public class ListStudentsQueryHandler : IRequestHandler<ListStudentsQuery, ErrorOr<PagedList<Student>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListStudentsQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public async ValueTask<ErrorOr<PagedList<Student>>> Handle(ListStudentsQuery request,
    CancellationToken cancellationToken) =>
    await _dbContext.Students
      .AsNoTracking()
      .OrderBy(student => student.Id)
      .ToPagedListAsync(request.PageNumber, request.PageSize, cancellationToken);
}
