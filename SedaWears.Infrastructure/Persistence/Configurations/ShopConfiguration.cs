using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000);
        
        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.IsActive);
    }
}
