using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database.Configurations;

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
