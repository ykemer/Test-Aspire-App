using Aspire_App.ApiService.Persistence;
using Contracts.Courses.Responses;
using Contracts.Students.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using IMapper = AutoMapper.IMapper;

namespace Aspire_App.ApiService.Features.Courses.GetCourse;

public class GetCourseEndpoint : EndpointWithoutRequest<ErrorOr<CourseResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCourseEndpoint(IMapper mapper, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/courses/{CourseId}");
        Policies("RequireAdministratorRole");
    }


    public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(
        CancellationToken ct)
    {
        var id = Route<Guid>("CourseId");

        var result = await _dbContext.Courses
            .Include(i => i.StudentEnrollments)
            .ThenInclude(x => x.Student)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(ct);

        if (result == null) return Error.NotFound(description: "Course not found");

        var course = _mapper.Map<CourseResponse>(result);
        var students = result.StudentEnrollments.Select(x => _mapper.Map<StudentResponse>(x.Student)).ToList();
        course.EnrolledStudents = students;

        return course;
    }
}