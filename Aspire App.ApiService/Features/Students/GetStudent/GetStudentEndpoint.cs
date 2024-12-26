using Aspire_App.ApiService.Persistence;
using Contracts.Students.Responses;
using FastEndpoints;
using IMapper = AutoMapper.IMapper;

namespace Aspire_App.ApiService.Features.Students.GetStudent;

public class GetStudentEndpoint : EndpointWithoutRequest<ErrorOr<StudentResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GetStudentEndpoint> _logger;
    private readonly IMapper _mapper;

    public GetStudentEndpoint(IMapper mapper, ILogger<GetStudentEndpoint> logger, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _logger = logger;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/students/{StudentId}");
        Policies("RequireAdministratorRole");
    }

    public override async Task<ErrorOr<StudentResponse>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("StudentId");

        var result = await _dbContext.Students.FindAsync(id, ct);

        if (result == null)
        {
            _logger.LogWarning("Student with id {StudentId} not found", id);
            return Error.NotFound(description: "Student not found");
        }

        return _mapper.Map<StudentResponse>(result);
    }
}