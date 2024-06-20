using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Requrests.Courses;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses;

public class EnrollToCourseEndpoint : Endpoint<StudentChangeEnrollRequest, IResult>
{
    private readonly IMediator _mediator;

    public EnrollToCourseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/courses/enroll");
        Policies("RequireUserRole");
    }

    public override async Task<IResult> ExecuteAsync(StudentChangeEnrollRequest request,
        CancellationToken cancellationToken)
    {
        HttpContext.Items.TryGetValue("UserId", out var userIdObj);
        var userId = Guid.Parse(userIdObj.ToString());

        await _mediator.Send(new EnrollToCourseCommand(request.CourseId, userId), cancellationToken);
        return Results.Ok();
    }
}