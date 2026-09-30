using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments;

namespace Service.Enrollments.Common.Database.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
  /// <summary>
  /// Unique index: a student can be enrolled in the same class only once.
  /// Code catches violations of this index by its name.
  /// </summary>
  public const string OneEnrollmentPerStudentAndClassIndex = "IX_Enrollments_StudentId_ClassId";

  public void Configure(EntityTypeBuilder<Enrollment> builder)
  {
    builder.HasKey(b => b.Id);

    // A class that still has enrollments cannot be deleted; the handlers check this first,
    // and the database refuses it as a last line of defense.
    builder.HasOne(enrollment => enrollment.Class)
      .WithMany(courseClass => courseClass.Enrollments)
      .HasForeignKey(enrollment => enrollment.ClassId)
      .OnDelete(DeleteBehavior.Restrict)
      .HasConstraintName("FK_Enrollments_Classes");

    builder.HasIndex(b => b.CourseId);
    builder.HasIndex(b => b.ClassId);
    builder.HasIndex(b => b.StudentId);

    builder.HasIndex(b => new { b.StudentId, b.ClassId })
      .IsUnique()
      .HasDatabaseName(OneEnrollmentPerStudentAndClassIndex);

    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.EnrollmentDateTime)
      .HasComment("Date and time when the student enrolled (UTC)")
      .HasColumnType("timestamp with time zone")
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
      .HasMaxLength(EnrollmentLimits.NameMaxLength)
      .IsRequired();

    builder.Property(b => b.StudentLastName)
      .HasComment("Student's last name")
      .HasMaxLength(EnrollmentLimits.NameMaxLength)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .HasComment("Date and time when the enrollment was created")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();

    builder.Property(b => b.UpdatedAt)
      .HasComment("Date and time when the enrollment was last updated")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();
  }
}
