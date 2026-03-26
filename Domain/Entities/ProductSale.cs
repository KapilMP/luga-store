using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class ProductSale : BaseAuditableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal DiscountedPrice { get; set; }
    public decimal? DiscountPercent { get; set; }

    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
}
