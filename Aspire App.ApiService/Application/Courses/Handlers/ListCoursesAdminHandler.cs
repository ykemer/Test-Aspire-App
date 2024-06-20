using Aspire_App.ApiService.Application.Courses.Queries;
using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Handlers;

public class ListCoursesAdminHandler : IRequestHandler<ListCoursesAdminQuery, PagedList<CourseResponse>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public ListCoursesAdminHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public Task<PagedList<CourseResponse>> Handle(ListCoursesAdminQuery request, CancellationToken cancellationToken)
    {
        var output = _courseRepository
            .GetAllAsync(cancellationToken)
            .OrderBy(i => i.Name)
            .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider);

        return Task.FromResult(PagedList<CourseResponse>.Create(output, request.Page, request.PageSize));
    }
}