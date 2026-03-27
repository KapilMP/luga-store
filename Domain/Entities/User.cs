using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class User : IdentityUser<int>, ISoftDelete, IAuditableEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Auditing
    public DateTime Created { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public int? LastModifiedBy { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }

    // Guest / Signup tracking
    public bool HasSignedUp { get; set; }

    // Active status
    public bool IsActive { get; set; } = true;

    // Profile
    public string? AvatarPath { get; set; }

    // Relationships
    public ICollection<Address> Addresses { get; set; } = [];
}
