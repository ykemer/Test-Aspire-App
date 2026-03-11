using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
  public void Configure(EntityTypeBuilder<Course> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasIndex(b => new { b.Name, b.Description })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");

    builder.HasMany(x => x.CourseClasses)
      .WithOne(x => x.Course)
      .HasForeignKey(x => x.CourseId);


    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.Name)
      .HasComment("Name of the course")
      .HasColumnType("text")
      .HasMaxLength(255)
      .IsRequired();

    builder.Property(b => b.Description)
      .HasComment("Description of the course")
      .HasColumnType("text")
      .HasMaxLength(2048)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .HasComment("Date and time when the class was created")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();

    builder.Property(b => b.UpdatedAt)
      .HasComment("Date and time when the class was last updated")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();


  }
}
