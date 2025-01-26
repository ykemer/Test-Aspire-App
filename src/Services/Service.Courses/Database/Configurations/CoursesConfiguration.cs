using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Entities;

namespace Service.Courses.Database.Configurations;

public class CoursesConfiguration : IEntityTypeConfiguration<Course>
{
  public void Configure(EntityTypeBuilder<Course> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasIndex(b => new { b.Name, b.Description })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");
  }
}
