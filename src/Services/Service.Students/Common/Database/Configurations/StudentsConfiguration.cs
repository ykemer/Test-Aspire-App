using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Students.Common.Database.Entities;

namespace Service.Students.Common.Database.Configurations;

public class StudentsConfiguration : IEntityTypeConfiguration<Student>
{
  public void Configure(EntityTypeBuilder<Student> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasIndex(b => new { b.FirstName, b.LastName, b.Email })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");

    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();


    builder.Property(b => b.FirstName)
      .HasComment("First name of the student")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(b => b.LastName)
      .HasComment("Last name of the student")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(b => b.Email)
      .HasComment("Email address of the student")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(b => b.DateOfBirth)
      .HasColumnType("date")
      .HasComment("Date of birth of the student")
      .IsRequired();

    builder.Property(b => b.EnrollmentsCount)
      .HasComment("Number of enrollments the student has")
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
