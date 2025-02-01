using Service.Courses.Entities;

namespace Service.Courses.Database;

public sealed class ApplicationDbContextInitializer
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<ApplicationDbContextInitializer> _logger;
  private const bool AddManyCourses = false;

  public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context)
  {
    _logger = logger;
    _context = context;
  }

  public async Task InitialiseAsync() => await MigrateAsync();

  private async Task MigrateAsync()
  {
    try
    {
      await _context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex,
        "An error occurred while trying to migrate the database.");
    }
  }

  public async Task SeedAsync()
  {
    try
    {
      await TrySeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database.");
    }
  }

  private async Task TrySeedAsync()
  {
    if (!await _context.Courses.AnyAsync())
    {
      var courseNames = new List<string> { "Physics", "Coding" };
      _context.Courses.Add(new Course
      {
        Id = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0", Name = "Math", Description = "Math course", EnrollmentsCount = 1
      });


      if (AddManyCourses)
      {
         for (var i = 1; i <= 5000; i++)
         {
           courseNames.Add($"Course-Created-By-Yakov-{i}");
         }
      }

      foreach (var name in courseNames)
      {
        _context.Courses.Add(new Course { Name = name, Description = $"{name} course", EnrollmentsCount = 0 });
      }
    }

    await _context.SaveChangesAsync();
  }
}
