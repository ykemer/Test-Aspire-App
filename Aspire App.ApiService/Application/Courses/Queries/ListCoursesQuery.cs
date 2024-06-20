using Aspire_App.ApiService.Application.Common;
using Aspire_App.ApiService.Application.Courses.Responses;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries;

public class ListCoursesQuery: PagedQuery, IRequest<PagedList<CourseResponse>>
{
    
}
