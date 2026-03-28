using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class PartnerManager : ISoftDelete, IAuditableEntity
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int ManagerId { get; set; }

    public User Partner { get; set; } = null!;
    public User Manager { get; set; } = null!;

    // Soft Delete / Auditing
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }
    public DateTime Created { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public int? LastModifiedBy { get; set; }
}