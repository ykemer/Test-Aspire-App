using Aspire_App.ApiService.Application.Courses.Queries;
using Aspire_App.ApiService.Application.Courses.Responses;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class GetCourseEndpoint : EndpointWithoutRequest<Results<Ok<CourseWithEnrolledStudentsResponse>, NotFound>>
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


    public override async Task<Results<Ok<CourseWithEnrolledStudentsResponse>, NotFound>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var id = Route<Guid>("CourseId");
        var getStudentResult = await _mediator.Send(new GetCourseQuery(id));
        if (getStudentResult == null) return TypedResults.NotFound();
        return TypedResults.Ok(getStudentResult);
    }
}