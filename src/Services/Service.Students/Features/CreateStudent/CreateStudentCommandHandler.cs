using Library.Database;

using Service.Students.Common.Database;

namespace Service.Students.Features.CreateStudent;

/// <summary>
/// Creates a student for a newly registered user.
/// <list type="bullet">
/// <item>Receiving the same user twice is fine: the second copy is ignored.</item>
/// <item>Two students cannot share an email (upper/lower case is ignored), enforced by a unique index.</item>
/// </list>
/// </summary>
public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateStudentCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public CreateStudentCommandHandler(ApplicationDbContext dbContext, ILogger<CreateStudentCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Created>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
  {
    var alreadyCreated = await _dbContext.Students.AnyAsync(s => s.Id == request.Id, cancellationToken);
    if (alreadyCreated)
    {
      _logger.LogInformation("Student {StudentId} already exists, ignoring the repeated message", request.Id);
      return Result.Created;
    }

    var lowerCaseEmail = request.Email.ToLowerInvariant();
    var emailIsTakenByAnotherStudent = await _dbContext.Students
      .AnyAsync(s => s.Id != request.Id && s.Email.ToLower() == lowerCaseEmail, cancellationToken);
    if (emailIsTakenByAnotherStudent)
    {
      _logger.LogWarning("Cannot create student {StudentId}: email is used by another student", request.Id);
      return StudentErrors.EmailAlreadyTaken(request.Email);
    }

    _dbContext.Students.Add(request.ToStudent(_timeProvider.GetUtcNow().UtcDateTime));

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation())
    {
      // Someone saved a student with the same id or email between our checks and our save.
      // If it is this very student, the same message was handled in parallel: that is a success.
      var sameStudentExists = await _dbContext.Students.AnyAsync(s => s.Id == request.Id, cancellationToken);
      if (sameStudentExists)
      {
        _logger.LogInformation("Student {StudentId} was created concurrently", request.Id);
        return Result.Created;
      }

      _logger.LogWarning("Cannot create student {StudentId}: email was taken concurrently", request.Id);
      return StudentErrors.EmailAlreadyTaken(request.Email);
    }

    _logger.LogInformation("Student {StudentId} was created", request.Id);
    return Result.Created;
  }
}
