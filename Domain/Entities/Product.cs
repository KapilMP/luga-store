using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    
    // Collaboration / Ownership
    public int? CreatorId { get; set; }
    public User? Creator { get; set; }
}
