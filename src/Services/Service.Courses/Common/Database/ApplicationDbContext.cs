using Service.Courses.Common.Database.Configurations;
using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database;

public class ApplicationDbContext : DbContext
{
  // Runs once per process (not on every new DbContext).
  // Keeps the old Npgsql DateTime handling that the rest of the system relies on.
  static ApplicationDbContext() => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public virtual DbSet<Course> Courses { get; set; }
  public virtual DbSet<Class> Classes { get; set; }
  public virtual DbSet<InboxMessage> InboxMessages { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(new CourseConfiguration());
    modelBuilder.ApplyConfiguration(new ClassConfiguration());
    modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
  }
}
