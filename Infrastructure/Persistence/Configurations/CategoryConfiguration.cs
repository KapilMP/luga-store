using LugaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(t => t.DisplayOrder);
        builder.HasIndex(t => t.PartnerId);
        builder.HasOne(t => t.Partner)
            .WithMany()
            .HasForeignKey(t => t.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
