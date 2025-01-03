using Contracts.Enrollments.Events;
using Service.Enrollments.AsyncDataServices;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;

public class EnrollStudentToCourseHandler: IRequestHandler<EnrollStudentToCourseCommand, ErrorOr<Created>>
{
    private readonly ILogger<EnrollStudentToCourseHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMessageBusClient _messageBusClient;

    public EnrollStudentToCourseHandler(ILogger<EnrollStudentToCourseHandler> logger, ApplicationDbContext dbContext, IMessageBusClient messageBusClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _messageBusClient = messageBusClient;
    }

    public async Task<ErrorOr<Created>> Handle(EnrollStudentToCourseCommand command, CancellationToken cancellationToken)
    {
        var existingEnrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == command.CourseId && e.StudentId == command.StudentId, cancellationToken: cancellationToken);
        if (existingEnrollment != null)
        {
            _logger.LogWarning("Student {StudentId} is already enrolled to course {CourseId}", command.StudentId, command.CourseId);
            return Error.Conflict("enrollment_service.enroll_student_to_course.already_enrolled",
                $"Student {command.StudentId} is already enrolled to course {command.CourseId}");
        }
        
        var enrollment = new Enrollment
        {
            CourseId = command.CourseId,
            StudentId = command.StudentId,
            StudentFirstName = command.FirstName,
            StudentLastName = command.LastName,
        };
        await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _messageBusClient.PublishStudentEnrolledEvent(new StudentEnrolledEvent
        {
            StudentId = command.StudentId,
            CourseId = command.CourseId
        });

        return Result.Created;
    }
}