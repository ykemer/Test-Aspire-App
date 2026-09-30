using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Common.Database.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
  /// <summary>
  /// Primary key: each request key can be stored only once. Code catches violations of it by this name.
  /// </summary>
  public const string PrimaryKeyName = "PK_IdempotencyRecords";

  public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
  {
    builder.HasKey(b => b.IdempotencyKey).HasName(PrimaryKeyName);

    builder.Property(b => b.IdempotencyKey)
      .HasComment("Client-supplied key identifying a single enroll/unenroll attempt")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(b => b.Operation)
      .HasComment("Which operation this key was recorded for (Enroll or Unenroll)")
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .HasComment("Date and time the operation was processed")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();
  }
}
