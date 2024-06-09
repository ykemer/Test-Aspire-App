using Aspire_App.ApiService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.EnsureCreated();
        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<StudentEnrollment> StudentEnrollments { get; set; }
    }
}
