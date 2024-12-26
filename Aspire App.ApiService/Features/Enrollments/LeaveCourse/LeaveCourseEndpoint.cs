using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.User;
using Contracts.Courses.Requests;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Features.Enrollments.LeaveCourse;

public class LeaveCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest,
    ErrorOr<Deleted>>
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IUserService _userService;

    public LeaveCourseEndpoint(IUserService userService, ApplicationDbContext dbContext)
    {
        _userService = userService;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/courses/leave");
        Policies("RequireUserRole");
        Claims("UserId");
    }

    public override async Task<ErrorOr<Deleted>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
        CancellationToken ct)
    {
        var userId = _userService.IsAdmin(User) ? request.StudentId : _userService.GetUserId(User);
        if (userId == Guid.Empty) return Error.Failure(description: "User not found");

        var enrollment = await _dbContext.StudentEnrollments
            .FirstOrDefaultAsync(x => x.CourseId == request.CourseId && x.StudentId == userId,
                ct);

        if (enrollment == null) return Error.Failure(description: "Enrollment for this student does not exist");

        _dbContext.StudentEnrollments.Remove(enrollment);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}