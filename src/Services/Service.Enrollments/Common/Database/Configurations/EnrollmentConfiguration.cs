using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
  public void Configure(EntityTypeBuilder<Enrollment> builder)
  {
    // Primary key should be the generated Id, not StudentId.
    builder.HasKey(b => b.Id);

    builder.HasOne(d => d.Class)
      .WithMany(p => p.Enrollments)
      .HasForeignKey(d => d.ClassId)
      .OnDelete(DeleteBehavior.ClientSetNull)
      .HasConstraintName("FK_Enrollments_Classes");

    builder.HasIndex(b => b.CourseId)
      .HasMethod("BTREE");

    builder.HasIndex(b => b.StudentId)
      .HasMethod("BTREE");


    builder.HasIndex(b => new { b.StudentFirstName, b.StudentLastName })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");

    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.CourseId)
      .HasComment("Course identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.ClassId)
      .HasComment("Class foreign key")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.StudentId)
      .HasComment("Student identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.StudentFirstName)
      .HasComment("Student's first name")
      .HasColumnType("text")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(b => b.StudentLastName)
      .HasComment("Student's last name")
      .HasColumnType("text")
      .HasMaxLength(100)
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
