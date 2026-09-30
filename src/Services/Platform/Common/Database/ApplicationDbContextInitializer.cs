using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

namespace Platform.Common.Database;

/// <summary>
/// Development helper: brings the database schema up to date, creates the roles,
/// an administrator and a sample student (the same student the Students service seeds).
/// If anything fails, the exception is not swallowed, so the service does not start in a broken state.
/// </summary>
public sealed class ApplicationDbContextInitializer
{
  public const string SeedPasswordSetting = "ADMIN_USER_PASSWORD";

  private const string AdministratorEmail = "admin@localhost";
  private const string SampleStudentEmail = "student@localhost";
  private const string SampleStudentId = "363fa2a4-70a8-4391-bc54-a8b5267fb68a";

  private readonly IConfiguration _configuration;
  private readonly ApplicationDbContext _context;
  private readonly ILogger<ApplicationDbContextInitializer> _logger;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly TimeProvider _timeProvider;
  private readonly UserManager<ApplicationUser> _userManager;

  public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
    IConfiguration configuration, TimeProvider timeProvider)
  {
    _logger = logger;
    _context = context;
    _userManager = userManager;
    _roleManager = roleManager;
    _configuration = configuration;
    _timeProvider = timeProvider;
  }

  public async Task MigrateAndSeedAsync()
  {
    _logger.LogInformation("Applying database migrations");
    await _context.Database.MigrateAsync();

    await SeedRolesAsync();
    await SeedUsersAsync();
  }

  private async Task SeedRolesAsync()
  {
    foreach (var role in Roles.All)
    {
      if (!await _roleManager.RoleExistsAsync(role))
      {
        EnsureSucceeded(await _roleManager.CreateAsync(new IdentityRole(role)), $"creating role {role}");
      }
    }
  }

  private async Task SeedUsersAsync()
  {
    if (await _userManager.FindByNameAsync(AdministratorEmail) is not null)
    {
      return;
    }

    _logger.LogInformation("Seeding the administrator and a sample student");
    var password = _configuration[SeedPasswordSetting]
                   ?? throw new InvalidOperationException($"{SeedPasswordSetting} is missing (see .env.dist).");
    var today = _timeProvider.GetUtcNow().UtcDateTime.Date;

    var administrator = new ApplicationUser
    {
      UserName = AdministratorEmail,
      Email = AdministratorEmail,
      FirstName = "John",
      LastName = "Doe",
      DateOfBirth = today.AddYears(-30),
      EmailConfirmed = true
    };
    EnsureSucceeded(await _userManager.CreateAsync(administrator, password), "creating the administrator");
    EnsureSucceeded(await _userManager.AddToRoleAsync(administrator, Roles.Administrator), "adding admin role");

    var student = new ApplicationUser
    {
      Id = SampleStudentId,
      UserName = SampleStudentEmail,
      Email = SampleStudentEmail,
      FirstName = "Marry",
      LastName = "Doe",
      DateOfBirth = today.AddYears(-25),
      EmailConfirmed = true
    };
    EnsureSucceeded(await _userManager.CreateAsync(student, password), "creating the sample student");
    EnsureSucceeded(await _userManager.AddToRoleAsync(student, Roles.User), "adding user role");
  }

  private static void EnsureSucceeded(IdentityResult result, string whatWasAttempted)
  {
    if (!result.Succeeded)
    {
      var errors = string.Join(", ", result.Errors.Select(error => error.Description));
      throw new InvalidOperationException($"Seeding failed while {whatWasAttempted}: {errors}");
    }
  }
}
