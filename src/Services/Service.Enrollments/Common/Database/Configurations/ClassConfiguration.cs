using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasKey(b => b.Id);

    // The Class ↔ Enrollment relationship is configured in EnrollmentConfiguration (one place only).

    // A shadow uint property named "xmin", marked as a row-version concurrency token, is detected by
    // Npgsql's model-finalizing convention and mapped to Postgres's own xmin system column (which the
    // database bumps on every write) instead of generating a real column — no application code needs
    // to manage it.
    builder.Property<uint>("xmin").IsRowVersion();

    builder.Property(b => b.Id)
      .HasComment("Unique identifier (same as in the Courses service)")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.CourseId)
      .HasComment("Course the class belongs to")
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

    builder.Property(b => b.EnrolledCount)
      .HasComment("Materialized count of active enrollments, kept in sync with the Enrollments table")
      .HasColumnType("integer")
      .HasDefaultValue(0)
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
