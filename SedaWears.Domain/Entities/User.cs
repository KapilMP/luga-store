using Microsoft.AspNetCore.Identity;
using SedaWears.Domain.Common;
using SedaWears.Domain.Enums;

namespace SedaWears.Domain.Entities;

public class User : IdentityUser<int>, ISoftDelete, IAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? CreatedById { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int? LastModifiedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; }

    // Profile
    public string? AvatarFileName { get; set; }

    // Relationships
    public virtual User? CreatedBy { get; set; }
    public virtual User? LastModifiedBy { get; set; }
    public ICollection<ShopOwner> OwnedShops { get; set; } = [];
    public ICollection<ShopManager> ManagedShops { get; set; } = [];
    public ICollection<Address> Addresses { get; set; } = [];
}
