using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SedaWears.Domain.Entities;

namespace SedaWears.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // IdentityUser properties like Id, Email, UserName are already configured by base.OnModelCreating(builder)
        // using IdentityDbContext. We only need to configure our custom added properties.

        builder.Property(u => u.FirstName)
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .HasMaxLength(50);

        builder.Property(u => u.AvatarFileName)
            .HasMaxLength(512);

        // Filtered Unique Index: Email + Role must be unique for non-deleted users
        builder.HasIndex(u => new { u.Email, u.Role })
            .HasFilter("\"IsDeleted\" = false") // Note: Use double quotes for Postgres case-sensitivity if needed, or check your provider
            .IsUnique();

        // Indexing common filter properties
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.IsDeleted);

        // If you want to automatically filter deleted users:
        // builder.HasQueryFilter(u => !u.IsDeleted); // This is already handled globally in ApplicationDbContext
    }
}
