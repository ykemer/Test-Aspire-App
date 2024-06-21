using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Requrests.Courses;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses;

public class LeaveCourseEndpoint : Endpoint<StudentChangeEnrollRequest,
    IResult>
{
    private readonly IMediator _mediator;

    public LeaveCourseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/courses/leave");
        Policies("RequireUserRole");
    }

    public override async Task<IResult> ExecuteAsync(StudentChangeEnrollRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        await _mediator.Send(new LeaveCourseCommand(request.CourseId, userId), cancellationToken);
        return Results.Ok();
    }
}