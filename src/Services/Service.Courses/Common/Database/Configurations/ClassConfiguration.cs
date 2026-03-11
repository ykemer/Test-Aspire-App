using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Common.Database.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasOne(d => d.Course)
      .WithMany(p => p.CourseClasses)
      .HasForeignKey(d => d.CourseId)
      .OnDelete(DeleteBehavior.Restrict)
      .HasConstraintName("FK_Classes_Courses");


    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.CourseId)
      .HasComment("Foreign key to the course")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.RegistrationDeadline)
      .HasComment("Deadline for students to register for the class")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.Property(b => b.CourseStartDate)
      .HasComment("Start date of the course")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.Property(b => b.CourseEndDate)
      .HasComment("End date of the course")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.Property(b => b.MaxStudents)
      .HasComment("Maximum number of students allowed in the class")
      .HasColumnType("integer")
      .IsRequired();

    builder.Property(b => b.TotalStudents)
      .HasComment("Total number of students currently enrolled in the class")
      .HasColumnType("integer")
      .IsRequired()
      .HasDefaultValue(0);

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
