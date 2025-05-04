using Contracts.Common;

using Service.Students.Entities;

namespace Service.Students.Features.ListStudent;

public class ListStudentsQueryHandler : IRequestHandler<ListStudentsQuery, ErrorOr<PagedList<Student>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListStudentsQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public ValueTask<ErrorOr<PagedList<Student>>> Handle(ListStudentsQuery request, CancellationToken cancellationToken)
  {
    var students = _dbContext.Students.AsNoTracking().OrderBy(student => student.Id).AsQueryable();
    var pagedStudents = PagedList<Student>.Create(students, request.PageNumber, request.PageSize);
    return ValueTask.FromResult<ErrorOr<PagedList<Student>>>(pagedStudents);
  }
}
