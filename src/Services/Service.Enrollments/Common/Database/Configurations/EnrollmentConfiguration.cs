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

    builder.HasIndex(b => new { b.StudentFirstName, b.StudentLastName })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");
  }
}
