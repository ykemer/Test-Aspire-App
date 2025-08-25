using Service.Enrollments.Database.Configurations;
using Service.Enrollments.Database.Entities;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Database;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public virtual DbSet<Enrollment> Enrollments { get; set; }
  public virtual DbSet<Class> Classes { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new CourseClassesConfiguration());
    modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
  }
}
