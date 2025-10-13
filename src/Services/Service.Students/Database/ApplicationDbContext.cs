using MassTransit;

using Service.Students.Database.Configurations;
using Service.Students.Database.Entities;

namespace Service.Students.Database;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public virtual DbSet<Student> Students { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.AddInboxStateEntity();
    modelBuilder.AddOutboxMessageEntity();
    modelBuilder.AddOutboxStateEntity();

    modelBuilder.ApplyConfiguration(new StudentsConfiguration());
  }
}
