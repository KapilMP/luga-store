using SedaWears.Domain.Common;
using SedaWears.Domain.Enums;

namespace SedaWears.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal ShippingCost { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Gender Gender { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }

    // Relationships
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<ProductSizeStock> SizeStocks { get; set; } = [];
    public ICollection<ProductSale> Sales { get; set; } = [];
}
