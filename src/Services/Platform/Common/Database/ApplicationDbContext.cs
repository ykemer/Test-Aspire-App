using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Platform.Common.Database.Configurations;
using Platform.Common.Database.Entities;

namespace Platform.Common.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  // Runs once per process (not on every new DbContext).
  // Keeps the old Npgsql DateTime handling that the rest of the system relies on.
  static ApplicationDbContext() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    builder.ApplyConfiguration(new RefreshTokenConfiguration());
  }
}
