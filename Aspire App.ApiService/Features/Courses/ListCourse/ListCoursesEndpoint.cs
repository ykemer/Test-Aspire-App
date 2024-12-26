using System.Security.Claims;
using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.User;
using AutoMapper.QueryableExtensions;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using FastEndpoints;
using Library.Pagination;
using Microsoft.EntityFrameworkCore;
using IMapper = AutoMapper.IMapper;


namespace Aspire_App.ApiService.Features.Courses.ListCourse;

public class ListCoursesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<CourseResponse>>>
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ListCoursesEndpoint(IMapper mapper, IUserService userService, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _userService = userService;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/courses/list");
        Policies("RequireUserRole");
        Claims("UserId");
        Claims(ClaimTypes.Role);
        ResponseCache(60);
    }

    public override Task<ErrorOr<PagedList<CourseResponse>>> ExecuteAsync(ListCoursesRequest query,
        CancellationToken ct)
    {
        IQueryable<CourseResponse> data;
        if (_userService.IsAdmin(User))
        {
            data = _dbContext.Courses
                .Include(i => i.StudentEnrollments)
                .ThenInclude(x => x.Student)
                .OrderBy(i => i.Name)
                .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider);
        }
        else
        {
            var userId = _userService.GetUserId(User);
            data = _dbContext.Courses
                .Include(i => i.StudentEnrollments.Where(x => x.StudentId == userId))
                .OrderBy(i => i.Name)
                .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider);
        }


        var result = PagedList<CourseResponse>.Create(data, query.Page, query.PageSize);

        return Task.FromResult<ErrorOr<PagedList<CourseResponse>>>(result);
    }
}