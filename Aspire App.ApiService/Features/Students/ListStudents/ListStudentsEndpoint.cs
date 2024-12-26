using Aspire_App.ApiService.Persistence;
using AutoMapper.QueryableExtensions;
using Contracts.Students.Requests;
using Contracts.Students.Responses;
using FastEndpoints;
using Library.Pagination;
using IMapper = AutoMapper.IMapper;

namespace Aspire_App.ApiService.Features.Students.ListStudents;

public class ListStudentsEndpoint : Endpoint<ListStudentsRequest, ErrorOr<PagedList<StudentResponse>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ListStudentsEndpoint(IMapper mapper, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/students/list");
        Policies("RequireAdministratorRole");
    }

    public override Task<ErrorOr<PagedList<StudentResponse>>> ExecuteAsync(ListStudentsRequest query,
        CancellationToken ct)
    {
        var data = _dbContext.Students.ProjectTo<StudentResponse>(_mapper.ConfigurationProvider);
        var result = PagedList<StudentResponse>.Create(data, query.Page, query.PageSize);
        return Task.FromResult<ErrorOr<PagedList<StudentResponse>>>(result);
    }
}