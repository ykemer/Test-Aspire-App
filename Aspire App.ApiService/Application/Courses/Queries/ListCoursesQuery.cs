using Aspire_App.ApiService.Application.Courses.Responses;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Queries;

public class ListCoursesQuery : ListCoursesGeneralQuery, IRequest<PagedList<CourseResponse>>
{
    public Guid StudentId { get; set; }


    public ListCoursesQuery(Guid studentId, ListCoursesGeneralQuery query)
    {
        StudentId = studentId;
        Page = query.Page;
        PageSize = query.PageSize;
    }
}