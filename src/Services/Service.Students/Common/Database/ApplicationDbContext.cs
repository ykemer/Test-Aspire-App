using Service.Students.Common.Database.Configurations;
using Service.Students.Common.Database.Entities;

namespace Service.Students.Common.Database;

public class ApplicationDbContext : DbContext
{
  // Runs once per process (not on every new DbContext).
  // Keeps the old Npgsql DateTime handling that the rest of the system relies on.
  static ApplicationDbContext() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public virtual DbSet<Student> Students { get; set; }
  public virtual DbSet<InboxMessage> InboxMessages { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(new StudentConfiguration());
    modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
  }
}
