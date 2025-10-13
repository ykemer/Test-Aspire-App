using Contracts.Common;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesQueryHandler : IRequestHandler<ListCoursesRequest, ErrorOr<PagedList<Course>>>
{
  private readonly ApplicationDbContext _dbContext;

  public ListCoursesQueryHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

  public ValueTask<ErrorOr<PagedList<Course>>> Handle(ListCoursesRequest request, CancellationToken cancellationToken)
  {
    var query = _dbContext.Courses.OrderBy(i => i.CreatedAt).AsQueryable();

    if (!request.ShowAll)
    {
      query = query.Where(x => x.CourseClasses.Any(cs =>
        request.EnrolledClasses.Contains(cs.Id) ||
        (cs.RegistrationDeadline >= DateTime.UtcNow && cs.TotalStudents < cs.MaxStudents)
      ));
    }

    if (!string.IsNullOrWhiteSpace(request.Query))
    {
      query = query
        .Where(x => EF.Functions.ToTsVector("english", x.Name + " " + x.Description)
          .Matches(EF.Functions.PhraseToTsQuery("english", request.Query))).Select(i => new CoursesDto
        {
          CreatedAt = i.CreatedAt,
          Description = i.Description,
          TotalStudents = i.TotalStudents,
          Id = i.Id,
          Name = i.Name,
          Rank = EF.Functions.ToTsVector("english", i.Name + " " + i.Description).Rank(
            EF.Functions.PhraseToTsQuery("english", request.Query))
        }).OrderByDescending(x => x.Rank);
    }


    var output = PagedList<Course>.Create(query, request.PageNumber, request.PageSize);
    return ValueTask.FromResult<ErrorOr<PagedList<Course>>>(output);
  }
}
