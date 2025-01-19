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

      query = query
        .Where(x => EF.Functions.ToTsVector("english", x.Name + " " + x.Description)
          .Matches(EF.Functions.PhraseToTsQuery("english", request.Query)))
        .Select(i => new CoursesDTO
        {
          CreatedAt = i.CreatedAt,
          Description = i.Description,
          EnrollmentsCount = i.EnrollmentsCount,
          Id = i.Id,
          Name = i.Name,
          Rank = EF.Functions.ToTsVector("english", i.Name + " " + i.Description).Rank(
            EF.Functions.PhraseToTsQuery("english", request.Query))
        }).OrderByDescending(x => x.Rank);
    }


    var output = PagedList<Course>.Create(query, request.PageNumber, request.PageSize);
    var b = timer.ElapsedMilliseconds;
    Console.WriteLine($"Query took {b}ms");
    return Task.FromResult<ErrorOr<PagedList<Course>>>(output);
  }
}
