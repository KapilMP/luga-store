using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SedaWears.Domain.Entities;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.UnsubscribeToken)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(n => n.Shop)
            .WithMany()
            .HasForeignKey(n => n.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique Enforcements: Email unique PER SHOP
        builder.HasIndex(n => new { n.ShopId, n.Email }).IsUnique();
        builder.HasIndex(n => n.UnsubscribeToken).IsUnique();
        builder.HasIndex(n => n.ShopId);
    }
}
