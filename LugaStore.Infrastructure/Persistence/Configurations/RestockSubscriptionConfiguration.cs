using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class RestockSubscriptionConfiguration : IEntityTypeConfiguration<RestockSubscription>
{
    public void Configure(EntityTypeBuilder<RestockSubscription> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Email).IsRequired().HasMaxLength(256);
        builder.Property(r => r.Size).IsRequired().HasMaxLength(16);

        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.ProductId);
        // Prevent duplicate subscriptions for same email + product + size
        builder.HasIndex(r => new { r.Email, r.ProductId, r.Size }).IsUnique();
    }
}
