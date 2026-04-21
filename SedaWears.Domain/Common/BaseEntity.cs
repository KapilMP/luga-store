using System;

namespace SedaWears.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    int? CreatedById { get; set; }
    DateTime? LastModifiedAt { get; set; }
    int? LastModifiedById { get; set; }
}

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity, ISoftDelete
{
    public DateTime CreatedAt { get; set; }
    public int? CreatedById { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int? LastModifiedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
