using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SedaWears.Domain.Entities;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ProductSaleConfiguration : IEntityTypeConfiguration<ProductSale>
{
    public void Configure(EntityTypeBuilder<ProductSale> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DiscountedPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.DiscountPercent).HasPrecision(5, 2);

        builder.HasOne(s => s.Product)
            .WithMany(p => p.Sales)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.ProductId, s.IsActive });
        builder.HasIndex(s => s.StartsAt);
        builder.HasIndex(s => s.EndsAt);
    }
}
