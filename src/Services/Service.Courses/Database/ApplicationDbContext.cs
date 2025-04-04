﻿using Service.Courses.Database.Configurations;
using Service.Courses.Entities;

namespace Service.Courses.Database;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) =>
    // For postgresql timestamp without time zone
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

  public virtual DbSet<Course> Courses { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) =>
    modelBuilder.ApplyConfiguration(new CoursesConfiguration());
}
