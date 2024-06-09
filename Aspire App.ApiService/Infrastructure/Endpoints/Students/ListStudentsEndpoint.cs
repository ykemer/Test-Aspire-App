using Aspire_App.ApiService.Application.Students.Commands;
using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Application.Students.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Students;

public class ListStudentsEndpoint : EndpointWithoutRequest<List<StudentResponse>>
{
    private readonly IMediator _mediator;

    public ListStudentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/students");
        AllowAnonymous();
    }

    public override async Task<List<StudentResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {

        return await _mediator.Send(new ListStudentsQuery());
    }
}