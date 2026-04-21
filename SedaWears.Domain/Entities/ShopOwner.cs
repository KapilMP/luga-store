using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class ShopOwner : BaseAuditableEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
}
