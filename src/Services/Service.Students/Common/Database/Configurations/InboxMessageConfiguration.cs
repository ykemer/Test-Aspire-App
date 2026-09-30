using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Service.Students.Common.Database.Entities;

namespace Service.Students.Common.Database.Configurations;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
  public const string PrimaryKeyName = "PK_InboxMessages";
  public const int MessageTypeMaxLength = 100;

  public void Configure(EntityTypeBuilder<InboxMessage> builder)
  {
    // The same message id + type can be stored only once. That is what makes handling idempotent.
    builder.HasKey(message => new { message.MessageId, message.MessageType }).HasName(PrimaryKeyName);

    builder.Property(message => message.MessageId)
      .HasComment("Id of the handled message");

    builder.Property(message => message.MessageType)
      .HasComment("Kind of the handled message")
      .HasMaxLength(MessageTypeMaxLength);

    builder.Property(message => message.ProcessedAt)
      .HasComment("Date and time when the message was handled")
      .HasColumnType("timestamp with time zone");
  }
}
