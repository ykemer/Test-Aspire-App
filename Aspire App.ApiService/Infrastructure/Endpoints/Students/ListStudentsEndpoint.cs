using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Application.Students.Responses;
using FastEndpoints;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Students;

public class ListStudentsEndpoint : Endpoint<ListStudentsQuery, PagedList<StudentResponse>>
{
    private readonly IMediator _mediator;

    public ListStudentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/students");
        Policies("RequireAdministratorRole");
    }

    public override async Task<PagedList<StudentResponse>> ExecuteAsync(ListStudentsQuery query, CancellationToken cancellationToken)
    {
        return await _mediator.Send(query);
    }
}