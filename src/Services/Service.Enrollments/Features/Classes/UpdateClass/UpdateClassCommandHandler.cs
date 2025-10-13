using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Classes.CreateClass;

namespace Service.Enrollments.Features.Classes.UpdateClass;

public class UpdateClassCommandHandler: IRequestHandler<UpdateClassCommand, ErrorOr<Updated>>
{

  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateClassCommandHandler> _logger;

  public UpdateClassCommandHandler(ApplicationDbContext dbContext, ILogger<CreateClassCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateClassCommand command, CancellationToken cancellationToken)
  {
      var existingClass = await _dbContext.Classes
          .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
      if (existingClass is null)
      {
        _logger.LogWarning("Class with id {Id} doesn't exist", command.Id);
        return Error.NotFound("class_service.update_class.not_found",
          $"Class with id {command.Id} doesn't exist");
      }


      existingClass.AddCommandData(command);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return Result.Updated;
  }
}
