using LugaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.Property(t => t.To)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(t => t.Subject)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(t => t.Body)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(t => t.ErrorMessage)
            .HasColumnType("text");

        builder.Property(t => t.MessageId)
            .HasMaxLength(64);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.To);
        builder.HasIndex(t => t.MessageId).IsUnique();
        builder.HasIndex(t => t.CreatedAt);
    }
}
