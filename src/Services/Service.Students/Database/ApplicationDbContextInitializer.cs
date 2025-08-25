using Service.Students.Database.Entities;

namespace Service.Students.Database;

public sealed class ApplicationDbContextInitializer
{
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
      await TrySeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database.");
    }
  }

  private async Task TrySeedAsync()
  {
    if (!await _context.Students.AnyAsync())
    {
      _context.Students.Add(new Student
      {
        Id = "363fa2a4-70a8-4391-bc54-a8b5267fb68a",
        Email = "student@localhost",
        FirstName = "Marry",
        LastName = "Doe",
        DateOfBirth = DateTime.Now.AddYears(-25),
        EnrollmentsCount = 0
      });

      await _context.SaveChangesAsync();
    }
  }
}
