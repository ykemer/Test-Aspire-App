using Service.Students.Common.Database;
using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.GetStudent;

/// <summary>
/// Returns one student.
/// </summary>
public class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, ErrorOr<Student>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetStudentQueryHandler> _logger;

  public GetStudentQueryHandler(ApplicationDbContext dbContext, ILogger<GetStudentQueryHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Student>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
  {
    var student = await _dbContext.Students
      .AsNoTracking()
      .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

    if (student is null)
    {
      _logger.LogWarning("Student {StudentId} was not found", request.StudentId);
      return StudentErrors.NotFound(request.StudentId);
    }

    return student;
  }
}
