using Aspire_App.ApiService.Application.Enrollment.Command;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Enrollments.Admin;

public class LeaveCourseByAdminEndpoint : Endpoint<LeaveCourseCommand,
    IResult>
{
    private readonly IMediator _mediator;

    public LeaveCourseByAdminEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/admin/courses/leave");
        Policies("RequireAdministratorRole");
    }

    public override async Task<IResult> ExecuteAsync(LeaveCourseCommand studentLeaveCourseCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(studentLeaveCourseCommand, cancellationToken);
        return result.MatchFirst(
            _ => Results.Ok(),
            ProblemHelper.Problem
        );
    }
}