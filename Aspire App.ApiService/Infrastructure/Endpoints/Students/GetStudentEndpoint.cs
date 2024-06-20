using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Application.Students.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Students;

public class GetStudentEndpoint : EndpointWithoutRequest<Results<Ok<StudentResponse>, NotFound>>
{
    private readonly IMediator _mediator;

    public GetStudentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/students/{StudentId}");
        Policies("RequireAdministratorRole");
    }

    public override async Task<Results<Ok<StudentResponse>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("StudentId");
        var getStudentResult = await _mediator.Send(new GetStudentQuery(id));
        if (getStudentResult == null) return TypedResults.NotFound();
        return TypedResults.Ok(getStudentResult);
    }
}