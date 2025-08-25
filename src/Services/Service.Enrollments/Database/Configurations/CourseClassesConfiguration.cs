using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Database.Entities;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Database.Configurations;

public class CourseClassesConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasKey(b => b.Id);
  }
}
