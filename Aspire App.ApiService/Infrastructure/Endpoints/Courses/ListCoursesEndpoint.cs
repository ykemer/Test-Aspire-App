using Aspire_App.ApiService.Application.Courses.Queries;
using Aspire_App.ApiService.Application.Courses.Responses;
using FastEndpoints;
using Library.Pagination;
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
    }

    public override async Task<IResult> ExecuteAsync(ListCoursesGeneralQuery query,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var output = await _mediator.Send(new ListCoursesQuery(userId, query), cancellationToken);
        return Results.Ok(output);
    }
}