using Aspire_App.ApiService.Application.Courses.Responses;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Queries;

public class ListCoursesAdminQuery : ListCoursesGeneralQuery, IRequest<PagedList<CourseResponse>>
{
}