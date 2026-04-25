using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class ShopMemberConfiguration : IEntityTypeConfiguration<ShopMember>
{
    public void Configure(EntityTypeBuilder<ShopMember> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.HasIndex(sm => new { sm.ShopId, sm.UserId })
            .IsUnique();

        builder.HasIndex(sm => sm.UserId);
        builder.HasIndex(sm => sm.IsActive);
        builder.HasIndex(sm => sm.IsInvitationAccepted);


        builder.HasOne(sm => sm.Shop)
            .WithMany(s => s.Members)
            .HasForeignKey(sm => sm.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sm => sm.User)
            .WithMany(u => u.ShopMemberships)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
