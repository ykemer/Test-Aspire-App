using Aspire_App.ApiService.Application.Courses.Queries;
using Aspire_App.ApiService.Application.Courses.Responses;
using FastEndpoints;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Admin;

public class ListAdminCoursesEndpoint : Endpoint<ListCoursesAdminQuery, PagedList<CourseResponse>>
{
    private readonly IMediator _mediator;

    public ListAdminCoursesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/admin/courses/list");
        Policies("RequireAdministratorRole");
    }

    public override async Task<PagedList<CourseResponse>> ExecuteAsync(ListCoursesAdminQuery query,
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(query);
    }
}