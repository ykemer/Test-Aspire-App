using Contracts.Students.Responses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Students;

public interface IStudentApiService
{
    Task<PagedList<StudentResponse>> GetStudentsListAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default);
}