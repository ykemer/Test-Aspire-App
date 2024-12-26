using Aspire_App.ApiService.Persistence;
using Contracts.Courses.Requests;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using IMapper = AutoMapper.IMapper;

namespace Aspire_App.ApiService.Features.Courses.CreateCourse;

public class CreateCourseEndpoint : Endpoint<CreateCourseRequest,
    ErrorOr<Created>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateCourseEndpoint(IMapper mapper, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/courses/create");
        Policies("RequireAdministratorRole");
    }

    public override async Task<ErrorOr<Created>> ExecuteAsync(CreateCourseRequest createCourseCommand,
        CancellationToken ct)
    {
        var course = _dbContext.Courses.FirstOrDefaultAsync(i => i.Name == createCourseCommand.Name, ct);
        if (course != null) return Error.Conflict(description: "Course already exists");

        var courseToSave = _mapper.Map<Course>(createCourseCommand);
        await _dbContext.Courses.AddAsync(courseToSave, ct);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Created;
    }
}