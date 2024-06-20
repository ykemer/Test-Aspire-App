using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Requrests.Courses;
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
        await _mediator.Send(studentLeaveCourseCommand, cancellationToken);
        return Results.Ok();
    }
}