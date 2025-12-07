using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database;

public sealed class ApplicationDbContextInitializer
{
  private const bool AddManyCourses = false;
  private readonly ApplicationDbContext _context;
  private readonly ILogger<ApplicationDbContextInitializer> _logger;

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
      // await TrySeedAsync();
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
      try
      {
        var courses = new List<Course>
        {
          new()
          {
            Name = "C#", Description = "C# course", TotalStudents = 0, Id = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0"
          },
          new()
          {
            Name = "Java", Description = "Java course", TotalStudents = 0, Id = "363fa2a4-70a8-4391-bc54-a8b5267fb68a"
          },
          new()
          {
            Name = "Python",
            Description = "Python course",
            TotalStudents = 0,
            Id = "e1a1c2b3-4d5e-6f7a-8b9c-0d1e2f3a4b5c"
          } // <-- Unique Id
        };

        await _context.Courses.AddRangeAsync(courses);

        var sharpClass = new Class
        {
          CourseId = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0",
          CourseStartDate = DateTime.Now.AddDays(3),
          CourseEndDate = DateTime.Now.AddDays(15),
          RegistrationDeadline = DateTime.Now.AddDays(5),
          MaxStudents = 100,
          Id = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0"
        };

        _context.Classes.Add(sharpClass);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while seeding the database.");
      }
    }
  }
}
