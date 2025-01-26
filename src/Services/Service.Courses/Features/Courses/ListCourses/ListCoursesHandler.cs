using System.Diagnostics;

using Contracts.Common;

using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesHandler : IRequestHandler<ListCoursesRequest, ErrorOr<PagedList<Course>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListCoursesHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public Task<ErrorOr<PagedList<Course>>> Handle(ListCoursesRequest request, CancellationToken cancellationToken)
  {
    var timer = Stopwatch.StartNew();
    var query = _dbContext.Courses.AsQueryable();
    if (!string.IsNullOrWhiteSpace(request.Query))
    {
      var tsQuery = EF.Functions.PhraseToTsQuery("english", request.Query);

      // First, filter using ToTsVector
      var filteredQuery = query
        .Where(x => EF.Functions.ToTsVector("english", x.Name + " " + x.Description)
          .Matches(tsQuery));

      // Then, project and order after bringing to client-side
      query = filteredQuery
        .Select(i => new CoursesDto
        {
          CreatedAt = i.CreatedAt,
          Description = i.Description,
          EnrollmentsCount = i.EnrollmentsCount,
          Id = i.Id,
          Name = i.Name,
          Rank = EF.Functions
            .ToTsVector("english", i.Name + " " + i.Description)
            .Rank(tsQuery)
        })
        .OrderByDescending(x => x.Rank);
    }


    var output = PagedList<Course>.Create(query, request.PageNumber, request.PageSize);
    var b = timer.ElapsedMilliseconds;
    Console.WriteLine($"Query took {b}ms");
    return Task.FromResult<ErrorOr<PagedList<Course>>>(output);
  }
}
