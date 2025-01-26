using Service.Enrollments.Entities;

namespace Service.Enrollments.Database;

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
    if (!await _context.Enrollments.AnyAsync())
    {
      _context.Enrollments.Add(new Enrollment
      {
        StudentId = "363fa2a4-70a8-4391-bc54-a8b5267fb68a",
        CourseId = "0b9de47c-fc66-4fb5-befe-5569b0fd6dd0",
        StudentFirstName = "Marry",
        StudentLastName = "Doe",
        EnrollmentDateTime = DateTime.Now
      });

      await _context.SaveChangesAsync();
    }
  }
}
