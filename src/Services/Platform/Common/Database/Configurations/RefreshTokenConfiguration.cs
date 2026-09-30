using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Platform.Common.Database.Entities;

namespace Platform.Common.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  /// <summary>A SHA-256 hash in Base64 is 44 characters long.</summary>
  public const int TokenHashLength = 44;

  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.HasKey(token => token.Id);

    builder.Property(token => token.Id)
      .HasComment("Unique identifier")
      .HasColumnType("uuid")
      .IsRequired();

    builder.Property(token => token.TokenHash)
      .HasComment("SHA-256 hash of the refresh token (the token itself is never stored)")
      .HasMaxLength(TokenHashLength)
      .IsRequired();

    builder.Property(token => token.UserId)
      .HasComment("User the refresh token belongs to")
      .HasMaxLength(256)
      .IsRequired();

    builder.Property(token => token.CreatedAt)
      .HasComment("Timestamp when the refresh token was created")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.Property(token => token.ExpiresAt)
      .HasComment("Timestamp when the refresh token expires")
      .HasColumnType("timestamp with time zone")
      .IsRequired();

    builder.HasIndex(token => token.TokenHash).IsUnique();

    builder.HasOne(token => token.User)
      .WithMany()
      .HasForeignKey(token => token.UserId)
      .OnDelete(DeleteBehavior.Cascade)
      .HasConstraintName("FK_RefreshTokens_ApplicationUsers");
  }
}
