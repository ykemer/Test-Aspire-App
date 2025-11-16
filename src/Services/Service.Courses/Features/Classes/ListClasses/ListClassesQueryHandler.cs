using Contracts.Common;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Classes.ListClasses;

public class ListClassesQueryHandler : IRequestHandler<ListClassesQuery, ErrorOr<PagedList<Class>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListClassesQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public ValueTask<ErrorOr<PagedList<Class>>> Handle(ListClassesQuery request, CancellationToken cancellationToken)
  {
    var query = _dbContext
      .Classes
      .AsNoTracking()
      .Where(courseClass => courseClass.CourseId == request.CourseId && (
        request.ShowAll ||
        (courseClass.RegistrationDeadline > DateTime.UtcNow && courseClass.TotalStudents < courseClass.MaxStudents)
        || request.EnrolledClasses.Contains(courseClass.Id)
      ));

    var output = PagedList<Class>.Create(query, request.PageNumber, request.PageSize);
    return ValueTask.FromResult<ErrorOr<PagedList<Class>>>(output);
  }
}
