using Aspire_App.ApiService.Application.Common;
using Aspire_App.ApiService.Application.Students.Responses;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries;

public class ListStudentsQuery: PagedQuery, IRequest<PagedList<StudentResponse>>
{
    
}
