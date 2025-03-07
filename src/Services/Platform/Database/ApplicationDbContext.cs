using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Platform.AsyncDataServices;
using Platform.AsyncDataServices.StateMachines;
using Platform.Entities;

namespace Platform.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<StudentEnrollmentsState>().HasIndex(x => x.CorrelationId);
    builder.Entity<StudentEnrollmentsState>().HasKey(x => x.EventId);
  }

  public DbSet<StudentEnrollmentsState> StudentEnrollmentStates { get; set; }
}
