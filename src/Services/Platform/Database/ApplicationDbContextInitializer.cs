using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Platform.Entities;

namespace Platform.Database;

public sealed class ApplicationDbContextInitializer : IApplicationDbContextInitializer
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<ApplicationDbContextInitializer> _logger;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
  {
    _logger = logger;
    _context = context;
    _userManager = userManager;
    _roleManager = roleManager;
  }

  public async Task InitialiseAsync() => await MigrateAsync();

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

  public async Task TrySeedAsync()
  {
    // Default roles
    var administratorRole = new IdentityRole("Administrator");
    var userRole = new IdentityRole("User");

    if (await _roleManager.Roles.AllAsync(r => r.Name != administratorRole.Name))
    {
      await _roleManager.CreateAsync(administratorRole);
      await _roleManager.CreateAsync(userRole);
    }


    // Default users
    var administrator = new ApplicationUser
    {
      UserName = "admin@localhost",
      Email = "admin@localhost",
      FirstName = "John",
      LastName = "Doe",
      DateOfBirth = DateTime.Now.AddYears(-30),
      EmailConfirmed = true
    };

    var student = new ApplicationUser
    {
      Id = "363fa2a4-70a8-4391-bc54-a8b5267fb68a",
      UserName = "student@localhost",
      Email = "student@localhost",
      FirstName = "Marry",
      LastName = "Doe",
      DateOfBirth = DateTime.Now.AddYears(-25),
      EmailConfirmed = true
    };

    if (await _userManager.Users.AllAsync(u => u.UserName != administrator.UserName))
    {
      await _userManager.CreateAsync(administrator, Environment.GetEnvironmentVariable("ADMIN_USER_PASSWORD")!);
      await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name }!);

      await _userManager.CreateAsync(student, Environment.GetEnvironmentVariable("ADMIN_USER_PASSWORD")!);
      await _userManager.AddToRolesAsync(student, new[] { userRole.Name }!);
    }


    await _context.SaveChangesAsync();
  }

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
}
