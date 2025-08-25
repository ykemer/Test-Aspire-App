using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Database.Entities;

namespace Service.Courses.Database.Configurations;

public class CourseClassesConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasOne(d => d.Course)
      .WithMany(p => p.CourseClasses)
      .HasForeignKey(d => d.CourseId)
      .OnDelete(DeleteBehavior.ClientSetNull)
      .HasConstraintName("FK_Classes_Courses");
  }
}
