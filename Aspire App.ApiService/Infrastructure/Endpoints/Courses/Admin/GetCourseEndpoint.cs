using Aspire_App.ApiService.Application.Courses.Queries;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class GetCourseEndpoint : EndpointWithoutRequest<IResult>
{
    private readonly IMediator _mediator;

    public GetCourseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/admin/courses/{CourseId}");
        Policies("RequireAdministratorRole");
    }


    public override async Task<IResult> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var id = Route<Guid>("CourseId");
        var getCourseResult = await _mediator.Send(new GetCourseQuery(id), cancellationToken);
        return getCourseResult.MatchFirst(
            course => Results.Ok(course),
            ProblemHelper.Problem
        );
    }
}