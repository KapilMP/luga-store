using System;

namespace LugaStore.Domain.Common;

public interface IAuditableEntity
{
    DateTime Created { get; set; }
    int? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    int? LastModifiedBy { get; set; }
}

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? Deleted { get; set; }
}

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity, ISoftDelete
{
    public DateTime Created { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public int? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }
}
