using Service.Students.Common.Database;

namespace Service.Students.Features.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateStudentCommandHandler> _logger;

  public CreateStudentCommandHandler(ApplicationDbContext dbContext, ILogger<CreateStudentCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Created>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
  {
    var existingStudent =
      await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);
    if (existingStudent != null)
    {
      _logger.LogWarning("Student with email {Email} already exists", request.Email);
      return Error.Conflict("students_service.create_student.already_exists",
        $"Student with email {request.Email} already exists");
    }

    var student = request.MapToStudent();
    await _dbContext.Students.AddAsync(student, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Created;
  }
}
