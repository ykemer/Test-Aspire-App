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
  }
}
