using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class ShopManager : BaseEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public int ManagerId { get; set; }
    public User Manager { get; set; } = null!;
}
