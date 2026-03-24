using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class User : IdentityUser<int>, ISoftDelete, IAuditableEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    // Auditing
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }

    // Relationship
}
