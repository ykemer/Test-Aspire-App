using Aspire_App.Web.Models.Students;
using Library.Pagination;

namespace Aspire_App.Web.Services.Students;

public interface IStudentApiService
{
    Task<PagedList<StudentListItem>> GetStudentsListAsync(int page, int pageSize = 10, CancellationToken cancellationToken = default);
}