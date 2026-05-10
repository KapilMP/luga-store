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

        builder.HasIndex(u => new { u.Email, u.Role })
            .IsUnique();

        // Indexing common filter properties
        builder.HasIndex(u => u.IsActive);
    }
}
