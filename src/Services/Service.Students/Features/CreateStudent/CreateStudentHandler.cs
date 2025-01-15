using Service.Students.Entities;

namespace Service.Students.Features.CreateStudent;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateStudentHandler> _logger;

  public CreateStudentHandler(ApplicationDbContext dbContext, ILogger<CreateStudentHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Created>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
  {
    Student? existingStudent =
      await _dbContext.Students.FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);
    if (existingStudent != null)
    {
      _logger.LogWarning("Student with email {Email} already exists", request.Email);
      return Error.Conflict("students_service.create_student.already_exists",
        $"Student with email {request.Email} already exists");
    }

    Student? student = new()
    {
      Id = request.Id,
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      DateOfBirth = request.DateOfBirth
    };
    await _dbContext.Students.AddAsync(student, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Created;
  }
}
