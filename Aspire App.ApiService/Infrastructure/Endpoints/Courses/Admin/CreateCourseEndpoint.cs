using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Application.Courses.Responses;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class CreateCourseEndpoint : Endpoint<CourseCreateCommand,
    CourseResponse>
{
    private readonly IMediator _mediator;

    public CreateCourseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/courses/create");
        Policies("RequireAdministratorRole");
    }

    public override async Task<CourseResponse> ExecuteAsync(CourseCreateCommand courseCreateCommand,
        CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(courseCreateCommand);
        return output;
    }
}