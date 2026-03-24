using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        // Collaborative Ownership Mapping
        builder.HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.SetNull); // Resilient deletion

        // Specialized Indices for the Clothing Store Ecosystem
        builder.HasIndex(p => p.CreatorId);
        builder.HasIndex(p => p.CategoryId);
    }
}
