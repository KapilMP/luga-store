using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ShopOwnerConfiguration : IEntityTypeConfiguration<ShopOwner>
{
    public void Configure(EntityTypeBuilder<ShopOwner> builder)
    {
        builder.HasKey(so => new { so.ShopId, so.OwnerId });

        builder.HasOne(so => so.Shop)
            .WithMany(s => s.Owners)
            .HasForeignKey(so => so.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(so => so.Owner)
            .WithMany(u => u.OwnedShops)
            .HasForeignKey(so => so.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
