using Service.Students.Entities;

namespace Service.Students.Features.GetStudent;

public class GetStudentHandler : IRequestHandler<GetStudentQuery, ErrorOr<Student>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetStudentHandler> _logger;

  public GetStudentHandler(ApplicationDbContext dbContext, ILogger<GetStudentHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Student>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
  {
    var student = await _dbContext.Students.FindAsync(request.StudentId, cancellationToken);
    if (student != null)
    {
      return student;
    }

    _logger.LogWarning("Student with id {StudentId} not found", request.StudentId);
    return Error.NotFound("students_service.get_student.not_found", $"Student with id {request.StudentId} not found");
  }
}
