using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class NewsletterConfiguration : IEntityTypeConfiguration<Newsletter>
{
    public void Configure(EntityTypeBuilder<Newsletter> builder)
    {
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.UnsubscribeToken)
            .IsRequired()
            .HasMaxLength(128);

        // Unique Enforcements
        builder.HasIndex(n => n.Email).IsUnique();
        builder.HasIndex(n => n.UnsubscribeToken).IsUnique();
    }
}
