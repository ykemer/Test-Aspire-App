using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database;

/// <summary>
/// Development helper: brings the database schema up to date and adds the sample class
/// (the same one the Courses service seeds).
/// If anything fails, the exception is not swallowed, so the service does not start on a broken database.
/// </summary>
public sealed class ApplicationDbContextInitializer
{
  private static readonly Guid s_csharpCourseId = Guid.Parse("0b9de47c-fc66-4fb5-befe-5569b0fd6dd0");
  private static readonly Guid s_csharpClassId = Guid.Parse("93f431c8-de1a-4456-a3f2-789fd4822626");

  private readonly ApplicationDbContext _context;
  private readonly ILogger<ApplicationDbContextInitializer> _logger;
  private readonly TimeProvider _timeProvider;

  public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context, TimeProvider timeProvider)
  {
    _logger = logger;
    _context = context;
    _timeProvider = timeProvider;
  }

  public async Task MigrateAndSeedAsync(CancellationToken cancellationToken = default)
  {
    _logger.LogInformation("Applying database migrations");
    await _context.Database.MigrateAsync(cancellationToken);

    await SeedAsync(cancellationToken);
  }

  private async Task SeedAsync(CancellationToken cancellationToken)
  {
    if (await _context.Classes.AnyAsync(cancellationToken))
    {
      return;
    }

    _logger.LogInformation("Seeding sample class");
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    _context.Classes.Add(new Class
    {
      Id = s_csharpClassId,
      CourseId = s_csharpCourseId,
      RegistrationDeadline = now.AddDays(3),
      CourseStartDate = now.AddDays(5),
      CourseEndDate = now.AddDays(15),
      MaxStudents = 100,
      CreatedAt = now,
      UpdatedAt = now
    });

    await _context.SaveChangesAsync(cancellationToken);
  }
}
