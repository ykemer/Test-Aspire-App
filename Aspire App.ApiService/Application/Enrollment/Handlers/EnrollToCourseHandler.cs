using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Application.Enrollment.Handlers;

public class EnrollToCourseHandler : IRequestHandler<EnrollToCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public EnrollToCourseHandler(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task Handle(EnrollToCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetAsync(request.CourseId, cancellationToken);
        if (course == null) throw new ArgumentException("Course does not exist");

        var existingEnrollment = await _enrollmentRepository.GetAllAsync(cancellationToken)
            .FirstOrDefaultAsync(i => i.StudentId == request.StudentId && i.CourseId == request.CourseId,
                cancellationToken);

        if (existingEnrollment != null) throw new Exception("Student already enrolled in this course");

        var enrollment = new StudentEnrollment
        {
            CourseId = request.CourseId,
            StudentId = request.StudentId
        };

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
    }
}