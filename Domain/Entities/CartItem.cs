using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

namespace LugaStore.Domain.Entities;

public class CartItem : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public ProductSize Size { get; set; }
    public int Quantity { get; set; }
}
