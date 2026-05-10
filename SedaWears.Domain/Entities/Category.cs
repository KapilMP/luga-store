using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    public int? ShopId { get; set; }
    public Shop? Shop { get; set; }
    public bool IsActive { get; set; } = false;

    // Relationships
    public ICollection<Product> Products { get; set; } = [];
}
