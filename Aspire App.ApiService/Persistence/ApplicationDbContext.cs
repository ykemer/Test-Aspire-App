using Aspire_App.ApiService.Features.Auth;
using Aspire_App.ApiService.Features.Courses;
using Aspire_App.ApiService.Features.Enrollments;
using Aspire_App.ApiService.Features.Students;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // For postgresql timestamp without time zone
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<StudentEnrollment> StudentEnrollments { get; set; }
}