using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database;

/// <summary>
/// Development helper: brings the database schema up to date and adds a few sample courses.
/// If anything fails, the exception is not swallowed, so the service does not start on a broken database.
/// </summary>
public sealed class ApplicationDbContextInitializer
{
  private static readonly Guid s_csharpCourseId = Guid.Parse("0b9de47c-fc66-4fb5-befe-5569b0fd6dd0");
  private static readonly Guid s_javaCourseId = Guid.Parse("363fa2a4-70a8-4391-bc54-a8b5267fb68a");
  private static readonly Guid s_pythonCourseId = Guid.Parse("e1a1c2b3-4d5e-6f7a-8b9c-0d1e2f3a4b5c");
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
    if (await _context.Courses.AnyAsync(cancellationToken))
    {
      return;
    }

    _logger.LogInformation("Seeding sample courses");
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    _context.Courses.AddRange(
      new Course { Id = s_csharpCourseId, Name = "C#", Description = "C# course", CreatedAt = now, UpdatedAt = now },
      new Course { Id = s_javaCourseId, Name = "Java", Description = "Java course", CreatedAt = now, UpdatedAt = now },
      new Course
      {
        Id = s_pythonCourseId, Name = "Python", Description = "Python course", CreatedAt = now, UpdatedAt = now
      });

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
