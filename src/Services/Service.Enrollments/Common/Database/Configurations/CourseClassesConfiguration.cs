using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database.Configurations;

public class CourseClassesConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasKey(b => b.Id);
  }
}
