using Service.Students.Entitites;

namespace Service.Students.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // For postgresql timestamp without time zone
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<Student> Students { get; set; }

}