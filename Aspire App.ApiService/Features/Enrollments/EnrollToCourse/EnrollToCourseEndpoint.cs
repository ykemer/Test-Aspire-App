using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.User;
using Contracts.Courses.Requests;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IUserService _userService;

    public EnrollToCourseEndpoint(
        IUserService userService, ApplicationDbContext dbContext)
    {
        _userService = userService;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/courses/enroll");
        Policies("RequireUserRole");
        Claims("UserId");
    }

    public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
        CancellationToken ct)
    {
        var userId = _userService.IsAdmin(User) ? request.StudentId : _userService.GetUserId(User);
        if (userId == Guid.Empty) return Error.Failure(description: "User not found");

        var course = await _dbContext.Courses.FindAsync(request.CourseId, ct);
        if (course == null) return Error.Failure(description: "Course does not exist");

        var existingEnrollment = await _dbContext.StudentEnrollments
            .FirstOrDefaultAsync(i => i.StudentId == userId && i.CourseId == request.CourseId,
                ct);

        if (existingEnrollment != null) Error.Conflict(description: "Student already enrolled in this course");


        var enrollment = new StudentEnrollment
        {
            CourseId = request.CourseId,
            StudentId = userId
        };

        await _dbContext.StudentEnrollments.AddAsync(enrollment, ct);
        await _dbContext.SaveChangesAsync(ct);
        return Result.Updated;
    }
}