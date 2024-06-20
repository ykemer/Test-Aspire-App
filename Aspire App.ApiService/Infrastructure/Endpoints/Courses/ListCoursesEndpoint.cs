using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Application.Students.Queries;
using FastEndpoints;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses;

public class ListCoursesEndpoint : Endpoint<ListCoursesQuery, PagedList<CourseResponse>>
{
    private readonly IMediator _mediator;

    public ListCoursesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/courses");
        Policies("RequireUserRole");
        Policies("RequireAdministratorRole");
    }

    public override async Task<PagedList<CourseResponse>> ExecuteAsync(ListCoursesQuery query, CancellationToken cancellationToken)
    {
        return await _mediator.Send(query);
    }
}