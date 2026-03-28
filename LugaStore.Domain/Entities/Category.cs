using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string Slug { get; set; } = null!;

    public int? PartnerId { get; set; }
    public User? Partner { get; set; }

    // Relationships
    public ICollection<Product> Products { get; set; } = [];
}
