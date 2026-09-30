using Service.Enrollments.Common.Database.Configurations;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database;

public class ApplicationDbContext : DbContext
{
  // Runs once per process (not on every new DbContext).
  // Keeps the old Npgsql DateTime handling that the rest of the system relies on.
  static ApplicationDbContext() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<Enrollment> Enrollments { get; set; }
  public DbSet<Class> Classes { get; set; }
  public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(new ClassConfiguration());
    modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
    modelBuilder.ApplyConfiguration(new IdempotencyRecordConfiguration());
  }
}
