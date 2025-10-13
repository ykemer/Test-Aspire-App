using MassTransit;

using Service.Enrollments.Common.Database.Configurations;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public DbSet<Enrollment> Enrollments { get; set; }
  public DbSet<Class> Classes { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();

    modelBuilder.ApplyConfiguration(new CourseClassesConfiguration());
    modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
  }
}
