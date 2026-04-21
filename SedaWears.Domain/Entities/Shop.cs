using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class Shop : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? BannerFileName { get; set; }
    public string? LogoFileName { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    // Relationships
    public ICollection<ShopOwner> Owners { get; set; } = [];
    public ICollection<ShopManager> Managers { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}
