using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Domain.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Application.Enrollment.Handlers;

public class LeaveCourseHandler : IRequestHandler<LeaveCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public LeaveCourseHandler(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task Handle(LeaveCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetAsync(request.CourseId, cancellationToken);
        if (course == null) throw new ArgumentException("Course does not exist");

        var enrollment = await _enrollmentRepository
            .GetAllAsync(cancellationToken)
            .FirstOrDefaultAsync(x => x.CourseId == request.CourseId && x.StudentId == request.StudentId,
                cancellationToken);

        if (enrollment == null) throw new ArgumentException("Enrollment does not exist");

        await _enrollmentRepository.DeleteAsync(enrollment.Id, cancellationToken);
    }
}