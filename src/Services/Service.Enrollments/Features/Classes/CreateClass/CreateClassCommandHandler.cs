using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Classes.CreateClass;

public class CreateClassCommandHandler: IRequestHandler<CreateClassCommand, ErrorOr<Created>>
{

  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<CreateClassCommandHandler> _logger;

  public CreateClassCommandHandler(ApplicationDbContext dbContext, ILogger<CreateClassCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Created>> Handle(CreateClassCommand command, CancellationToken cancellationToken)
  {
      var existingClass = await _dbContext.Classes
          .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
      if (existingClass is not null)
      {
        _logger.LogWarning("Class with id {Id} already exists", command.Id);
        return Error.Conflict("class_service.create_class.already_exists",
          $"Class with id {command.Id} already exists");
      }


      var newClass = command.MapToClass();
      _dbContext.Classes.Add(newClass);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return Result.Created;
  }
}
