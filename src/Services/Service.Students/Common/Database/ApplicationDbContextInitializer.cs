using Service.Students.Common.Database.Entities;

namespace Service.Students.Common.Database;

/// <summary>
/// Development helper: brings the database schema up to date and adds a sample student.
/// If anything fails, the exception is not swallowed, so the service does not start on a broken database.
/// </summary>
public sealed class ApplicationDbContextInitializer
{
  private static readonly Guid s_sampleStudentId = Guid.Parse("363fa2a4-70a8-4391-bc54-a8b5267fb68a");

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
    if (await _context.Students.AnyAsync(cancellationToken))
    {
      return;
    }

    _logger.LogInformation("Seeding sample student");
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    _context.Students.Add(new Student
    {
      Id = s_sampleStudentId,
      Email = "student@localhost",
      FirstName = "Marry",
      LastName = "Doe",
      DateOfBirth = now.Date.AddYears(-25),
      CreatedAt = now,
      UpdatedAt = now
    });

    await _context.SaveChangesAsync(cancellationToken);
  }
}
