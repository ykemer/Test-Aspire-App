using Aspire_App.ApiService.Application.Enrollment.Command;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class EnrollToCourseByAdminEndpoint : Endpoint<EnrollToCourseCommand,
    IResult>
{
    private readonly IMediator _mediator;

    public EnrollToCourseByAdminEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/admin/courses/enroll");
        Policies("RequireAdministratorRole");
    }

    public override async Task<IResult> ExecuteAsync(EnrollToCourseCommand studentLeaveCourseCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(studentLeaveCourseCommand, cancellationToken);
        return result.MatchFirst(
            _ => Results.Ok(),
            ProblemHelper.Problem
        );
    }
}