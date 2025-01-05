using Contracts.Students.Events;
using Service.Students.AsyncDataServices;

namespace Service.Students.Features.DeleteStudent;

public class DeleteStudentHandler: IRequestHandler<DeleteStudentCommand, ErrorOr<Deleted>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DeleteStudentHandler> _logger;
    private readonly IMessageBusClient _messageBusClient;

    public DeleteStudentHandler(ApplicationDbContext dbContext, ILogger<DeleteStudentHandler> logger, IMessageBusClient messageBusClient)
    {
        _dbContext = dbContext;
        _logger = logger;
        _messageBusClient = messageBusClient;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(i => i.Id == request.StudentId, cancellationToken);
        if (student == null)
        {
            _logger.LogWarning("Student {StudentId} not found", request.StudentId);
            return Error.NotFound("student_service.delete_student.student_not_found", $"Student {request.StudentId} not found");
        }
        _dbContext.Remove(student);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _messageBusClient.PublishStudentDeletedEvent(new StudentDeletedEvent
        {
            StudentId = request.StudentId
        });
        return Result.Deleted;
    }
}