using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // IdentityUser properties like Id, Email, UserName are already configured by base.OnModelCreating(builder)
        // using IdentityDbContext. We only need to configure our custom added properties.

        builder.Property(u => u.FirstName)
            .HasMaxLength(64);

        builder.Property(u => u.LastName)
            .HasMaxLength(64);

        builder.Property(u => u.AvatarPath)
            .HasMaxLength(512);

        // Indexing common filter properties
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.IsDeleted);

        // If you want to automatically filter deleted users:
        // builder.HasQueryFilter(u => !u.IsDeleted); // This is already handled globally in ApplicationDbContext
    }
}
