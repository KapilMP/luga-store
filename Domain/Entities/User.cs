using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class User : IdentityUser<int>, ISoftDelete, IAuditableEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public override string? Email
    {
        get => base.Email;
        set => base.Email = base.UserName = value;
    }

    // Auditing
    public DateTime Created { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public int? LastModifiedBy { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }

    // Active status
    public bool IsActive { get; set; } = true;

    // Partner manager assignments
    public ICollection<PartnerManager> PartnerAssignments { get; set; } = [];
    public ICollection<PartnerManager> ManagerAssignments { get; set; } = [];

    // Profile
    public string? AvatarPath { get; set; }

    // Relationships
    public ICollection<Address> Addresses { get; set; } = [];
}
