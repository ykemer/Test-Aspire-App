using Contracts.Students.Events;

using MassTransit;

using Service.Students.Common.Database;

namespace Service.Students.Features.DeleteStudent;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteStudentCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteStudentCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteStudentCommandHandler> logger,
    IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
  {
    var student = await _dbContext.Students.FirstOrDefaultAsync(i => i.Id == request.StudentId, cancellationToken);
    if (student == null)
    {
      _logger.LogWarning("Student {StudentId} not found", request.StudentId);
      return Error.NotFound("student_service.delete_student.student_not_found",
        $"Student {request.StudentId} not found");
    }

    if(student.EnrollmentsCount > 0)
    {
      _logger.LogWarning("Cannot delete student {StudentId} with active enrollments", request.StudentId);
      return Error.Validation("student_service.delete_student.student_has_enrollments",
        $"Cannot delete student {request.StudentId} with active enrollments");
    }

    _dbContext.Remove(student);
    await _dbContext.SaveChangesAsync(cancellationToken);
    await _publishEndpoint.Publish(new StudentDeletedEvent { StudentId = request.StudentId }, cancellationToken);
    return Result.Deleted;
  }
}
