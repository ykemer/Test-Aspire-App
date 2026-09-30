using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.UpdateClass;

/// <summary>
/// Updates the copy of a class after it was changed in the Courses service.
/// If the class is not here yet (its "created" message is still on the way), this fails and the
/// message is retried later.
/// </summary>
public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UpdateClassCommandHandler(ApplicationDbContext dbContext, ILogger<UpdateClassCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateClassCommand command, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
    if (courseClass is null)
    {
      _logger.LogWarning("Cannot update class {ClassId} because it was not copied yet", command.Id);
      return ClassErrors.NotFound(command.Id);
    }

    courseClass.ApplyUpdate(command, _timeProvider.GetUtcNow().UtcDateTime);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Class {ClassId} was updated", command.Id);
    return Result.Updated;
  }
}
