using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ShopManagerConfiguration : IEntityTypeConfiguration<ShopManager>
{
    public void Configure(EntityTypeBuilder<ShopManager> builder)
    {
        builder.HasKey(sm => new { sm.ShopId, sm.ManagerId });

        builder.HasOne(sm => sm.Shop)
            .WithMany(s => s.Managers)
            .HasForeignKey(sm => sm.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sm => sm.Manager)
            .WithMany(u => u.ManagedShops)
            .HasForeignKey(sm => sm.ManagerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
