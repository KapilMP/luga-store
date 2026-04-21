using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        
        builder.HasIndex(t => t.Name).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(t => t.Slug).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(t => t.IsActive);
        builder.HasIndex(t => t.IsDeleted).HasFilter("\"IsDeleted\" = false");
    }
}
