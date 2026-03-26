using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

namespace LugaStore.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal ShippingCost { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }

    // Collaboration / Ownership
    public int? CreatorId { get; set; }
    public User? Creator { get; set; }

    // Relationships
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<ProductSizeStock> SizeStocks { get; set; } = [];
    public ICollection<ProductSale> Sales { get; set; } = [];
}
