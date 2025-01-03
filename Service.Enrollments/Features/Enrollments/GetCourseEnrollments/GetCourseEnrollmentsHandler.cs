using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public class GetCourseEnrollmentsHandler: IRequestHandler<GetCourseEnrollmentsRequest, ErrorOr<List<Enrollment>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetCourseEnrollmentsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<Enrollment>>> Handle(GetCourseEnrollmentsRequest request, CancellationToken cancellationToken)
    {
        var enrollments = await _dbContext.Enrollments
            .Where(e => e.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        return enrollments;
    }
}