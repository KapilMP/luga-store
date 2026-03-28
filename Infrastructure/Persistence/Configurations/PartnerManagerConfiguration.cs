using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class PartnerManagerConfiguration : IEntityTypeConfiguration<PartnerManager>
{
    public void Configure(EntityTypeBuilder<PartnerManager> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.HasOne(pm => pm.Partner)
            .WithMany(p => p.AssignedManagerships)
            .HasForeignKey(pm => pm.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.Manager)
            .WithMany(m => m.ManagedPartnerships)
            .HasForeignKey(pm => pm.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pm => pm.PartnerId);
        builder.HasIndex(pm => pm.ManagerId);

        // Manager can only be assigned once per partner (unless soft-deleted)
        builder.HasIndex(pm => new { pm.PartnerId, pm.ManagerId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
