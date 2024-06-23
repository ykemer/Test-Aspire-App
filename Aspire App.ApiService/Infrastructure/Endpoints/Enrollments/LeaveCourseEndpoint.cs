using System.Security.Claims;
using Aspire_App.ApiService.Application.Enrollment.Command;
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
        Claims("UserId");
    }

    public override async Task<IResult> ExecuteAsync(StudentChangeEnrollRequest request,
        CancellationToken cancellationToken)
    {
        Guid.TryParse(User.FindFirstValue("UserId"), out var userId);
        var result = await _mediator.Send(new LeaveCourseCommand(request.CourseId, userId), cancellationToken);
        return result.MatchFirst(
            _ => Results.Ok(),
            ProblemHelper.Problem
        );
    }
}