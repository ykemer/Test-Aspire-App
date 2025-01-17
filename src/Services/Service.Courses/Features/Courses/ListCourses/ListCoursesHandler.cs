using Contracts.Common;

using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesHandler : IRequestHandler<ListCoursesRequest, ErrorOr<PagedList<Course>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListCoursesHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public Task<ErrorOr<PagedList<Course>>> Handle(ListCoursesRequest request, CancellationToken cancellationToken)
  {
    var query = _dbContext.Courses.AsQueryable();
    if (!string.IsNullOrWhiteSpace(request.Query))
    {
      // TODO - implement Elasticsearch
      query = query.Where(i => i.Name.Contains(request.Query) || i.Description.Contains(request.Query))
        .OrderBy(i => i.Name);
    }

    var output = PagedList<Course>.Create(query, request.PageNumber, request.PageSize);
    return Task.FromResult<ErrorOr<PagedList<Course>>>(output);
  }
}
