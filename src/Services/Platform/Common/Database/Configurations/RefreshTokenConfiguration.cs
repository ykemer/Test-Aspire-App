using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Platform.Common.Database.Entities;

namespace Platform.Common.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.HasKey(rt => rt.Id);

    builder.Property(b => b.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(rt => rt.Token)
      .HasMaxLength(256)
      .HasComment("Refresh token string")
      .HasColumnType("text")
      .IsRequired();

    builder.Property(rt => rt.UserId)
      .HasMaxLength(256)
      .HasComment("User Id associated with the refresh token")
      .HasColumnType("text")
      .IsRequired();

    builder.Property(rt => rt.CreatedAt)
      .HasComment("Timestamp when the refresh token was created")
      .HasColumnType("timestamp with time zone")
      .HasDefaultValueSql("CURRENT_TIMESTAMP")
      .IsRequired();


    builder.Property(rt => rt.ExpiresAt)
      .HasComment("Timestamp when the refresh token expires")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.Property(rt => rt.IsValid)
      .HasComment("Indicates whether the refresh token is valid")
      .HasColumnType("boolean")
      .IsRequired()
      .HasDefaultValue(true);

    builder
      .HasIndex(r => r.Token)
      .IsUnique();

    builder.HasOne(rt => rt.User)
      .WithMany()
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade)
      .HasConstraintName("FK_RefreshTokens_ApplicationUsers");
  }
}
