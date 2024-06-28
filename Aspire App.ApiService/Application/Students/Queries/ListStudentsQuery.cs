using Aspire_App.ApiService.Application.Common;
using Contracts.Students.Responses;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries;

public class ListStudentsQuery : PagedQuery, IRequest<PagedList<StudentResponse>>
{
}