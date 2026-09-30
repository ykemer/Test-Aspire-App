using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;

namespace Service.Courses.Common.Database.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
  /// <summary>
  /// Language used by the full-text search index. Queries must use the same value, or the index is not used.
  /// </summary>
  public const string SearchLanguage = "english";

  // Note: there is also a unique index on lower("Name") (two courses cannot share a name, ignoring case).
  // EF Core cannot describe an index on an expression, so the HardenCoursesSchema migration creates it with SQL.
  public void Configure(EntityTypeBuilder<Course> builder)
  {
    builder.HasKey(b => b.Id);
    builder.HasIndex(b => new { b.Name, b.Description })
      .HasMethod("GIN")
      .IsTsVectorExpressionIndex(SearchLanguage);

    builder.HasMany(x => x.CourseClasses)
      .WithOne(x => x.Course)
      .HasForeignKey(x => x.CourseId);

    // See ClassConfiguration for why this maps to Postgres's own xmin system column.
    builder.Property<uint>("xmin").IsRowVersion();

    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.Name)
      .HasComment("Name of the course")
      .HasMaxLength(CourseLimits.NameMaxLength)
      .IsRequired();

    builder.Property(b => b.Description)
      .HasComment("Description of the course")
      .HasMaxLength(CourseLimits.DescriptionMaxLength)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .HasComment("Date and time when the course was created")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();

    builder.Property(b => b.UpdatedAt)
      .HasComment("Date and time when the course was last updated")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();
  }
}
