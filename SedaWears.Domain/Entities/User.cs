using Microsoft.AspNetCore.Identity;
using SedaWears.Domain.Common;
using SedaWears.Domain.Enums;

namespace SedaWears.Domain.Entities;

public class User : IdentityUser<int>, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserRole Role { get; set; }

    // Profile
    public string? AvatarFileName { get; set; }

    // Relationships
    public ICollection<ShopOwner> OwnedShops { get; set; } = [];
    public ICollection<ShopManager> ManagedShops { get; set; } = [];
    public ICollection<Address> Addresses { get; set; } = [];
}
