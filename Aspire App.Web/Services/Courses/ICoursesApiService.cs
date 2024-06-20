using Aspire_App.Web.Models.Courses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Courses;

public interface ICoursesApiService
{
    Task<PagedList<CoursesListItem>> GetCoursesListAsync(int page, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<CoursesListItem> GetCourse(Guid guid, CancellationToken cancellationToken = default);
}