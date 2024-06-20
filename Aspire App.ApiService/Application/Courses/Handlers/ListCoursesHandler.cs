using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Handlers;

public class ListCoursesHandler : IRequestHandler<ListCoursesQuery, PagedList<CourseResponse>>
{
    private readonly IMapper _mapper;
    private readonly ICourseRepository _courseRepository;

    public ListCoursesHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public Task<PagedList<CourseResponse>> Handle(ListCoursesQuery request, CancellationToken cancellationToken)
    {
        var output =  _courseRepository
            .GetAllAsync(cancellationToken)
            .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider);

        return Task.FromResult(PagedList<CourseResponse>.Create(output, request.Page, request.PageSize));
    }
}