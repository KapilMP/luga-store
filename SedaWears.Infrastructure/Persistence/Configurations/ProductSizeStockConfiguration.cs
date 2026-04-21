using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SedaWears.Domain.Entities;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ProductSizeStockConfiguration : IEntityTypeConfiguration<ProductSizeStock>
{
    public void Configure(EntityTypeBuilder<ProductSizeStock> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Size)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(s => s.Stock).IsRequired();

        builder.HasOne(s => s.Product)
            .WithMany(p => p.SizeStocks)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // One stock record per size per product
        builder.HasIndex(s => new { s.ProductId, s.Size }).IsUnique();
    }
}
