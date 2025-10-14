using MassTransit;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Platform.Common.Database.Entities;
using Platform.Common.StateMachines;

namespace Platform.Common.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public DbSet<StudentEnrollState> StudentEnrollStates { get; set; }

  public DbSet<StudentUnenrollState> StudentUnenrollStates { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.AddInboxStateEntity();
    builder.AddOutboxMessageEntity();
    builder.AddOutboxStateEntity();

    builder.Entity<StudentEnrollState>().HasIndex(x => x.CorrelationId);
    builder.Entity<StudentEnrollState>().HasKey(x => x.EventId);

    builder.Entity<StudentUnenrollState>().HasIndex(x => x.CorrelationId);
    builder.Entity<StudentUnenrollState>().HasKey(x => x.EventId);


    builder.Entity<RefreshToken>()
      .HasIndex(r => r.Token)
      .IsUnique();
  }
}
