using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Students.Common.Database.Entities;
using Service.Students.Features;

namespace Service.Students.Common.Database.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
  // Note: there is also a unique index on lower("Email") (two students cannot share an email, ignoring case).
  // EF Core cannot describe an index on an expression, so the HardenStudentsSchema migration creates it with SQL.
  public void Configure(EntityTypeBuilder<Student> builder)
  {
    builder.HasKey(b => b.Id);

    builder.Property(b => b.Id)
      .HasComment("Unique identifier (same as the user id in Platform)")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.FirstName)
      .HasComment("First name of the student")
      .HasMaxLength(StudentLimits.NameMaxLength)
      .IsRequired();

    builder.Property(b => b.LastName)
      .HasComment("Last name of the student")
      .HasMaxLength(StudentLimits.NameMaxLength)
      .IsRequired();

    builder.Property(b => b.Email)
      .HasComment("Email address of the student")
      .HasMaxLength(StudentLimits.EmailMaxLength)
      .IsRequired();

    builder.Property(b => b.DateOfBirth)
      .HasColumnType("date")
      .HasComment("Date of birth of the student")
      .IsRequired();

    builder.Property(b => b.EnrollmentsCount)
      .HasComment("Number of classes the student is enrolled in")
      .HasDefaultValue(0)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .HasComment("Date and time when the student was created")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();

    builder.Property(b => b.UpdatedAt)
      .HasComment("Date and time when the student was last updated")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();
  }
}
