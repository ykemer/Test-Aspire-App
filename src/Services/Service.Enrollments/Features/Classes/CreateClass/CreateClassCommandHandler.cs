using Library.Database;

using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.CreateClass;

/// <summary>
/// Saves a copy of a class that was created in the Courses service.
/// Receiving the same class twice is fine: the second copy is ignored.
/// </summary>
public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public CreateClassCommandHandler(ApplicationDbContext dbContext, ILogger<CreateClassCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Created>> Handle(CreateClassCommand command, CancellationToken cancellationToken)
  {
    var alreadyCopied = await _dbContext.Classes.AnyAsync(c => c.Id == command.Id, cancellationToken);
    if (alreadyCopied)
    {
      _logger.LogInformation("Class {ClassId} was already copied, ignoring the repeated message", command.Id);
      return Result.Created;
    }

    _dbContext.Classes.Add(command.ToClass(_timeProvider.GetUtcNow().UtcDateTime));

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation())
    {
      // The same message was handled in parallel and saved first. The class is there, which is all we need.
      _logger.LogInformation("Class {ClassId} was copied concurrently", command.Id);
      return Result.Created;
    }

    _logger.LogInformation("Class {ClassId} of course {CourseId} was copied", command.Id, command.CourseId);
    return Result.Created;
  }
}
