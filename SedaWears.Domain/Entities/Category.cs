using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string Slug { get; set; } = null!;

    public int? ShopId { get; set; }
    public Shop? Shop { get; set; }

    // Relationships
    public ICollection<Product> Products { get; set; } = [];
}
