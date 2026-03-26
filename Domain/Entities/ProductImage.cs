using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ImagePath { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
