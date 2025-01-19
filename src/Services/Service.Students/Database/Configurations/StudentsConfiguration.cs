using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Students.Entities;

namespace Service.Students.Database.Configurations;

public class StudentsConfiguration : IEntityTypeConfiguration<Student>
{
  public void Configure(EntityTypeBuilder<Student> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasIndex(b => new { b.FirstName, b.LastName, b.Email})
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex("english");
  }
}
