using System.Security.Claims;
using Aspire_App.ApiService.Application.Courses.Queries;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses;

public class ListCoursesEndpoint : Endpoint<ListCoursesGeneralQuery, IResult>
{
    private readonly IMediator _mediator;

    public ListCoursesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/courses/list");
        Policies("RequireUserRole");
        Claims("UserId");
    }

    public override async Task<IResult> ExecuteAsync(ListCoursesGeneralQuery query,
        CancellationToken cancellationToken)
    {
        Guid.TryParse(User.FindFirstValue("UserId"), out var userId);
        var output = await _mediator.Send(new ListCoursesQuery(userId, query), cancellationToken);
        return Results.Ok(output);
    }
}