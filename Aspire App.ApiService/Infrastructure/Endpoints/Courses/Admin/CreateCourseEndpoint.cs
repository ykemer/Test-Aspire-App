using Aspire_App.ApiService.Application.Courses.Command;
using ErrorOr;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class CreateCourseEndpoint : Endpoint<CourseCreateCommand,
    IResult>
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

    public override async Task<IResult> ExecuteAsync(CourseCreateCommand courseCreateCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(courseCreateCommand);
        return result.MatchFirst(
            course => Results.Ok(course),
            ProblemHelper.Problem
        );
    }
    
  
}


