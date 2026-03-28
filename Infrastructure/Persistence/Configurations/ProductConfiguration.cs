using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.Property(p => p.ShippingCost).HasPrecision(18, 2);
        builder.Property(p => p.Description).HasMaxLength(2048);

        builder.HasIndex(p => p.Gender);
        builder.HasIndex(p => p.IsFeatured);
        builder.HasIndex(p => p.IsNew);
    }
}
