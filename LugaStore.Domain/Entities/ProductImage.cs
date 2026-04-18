using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
    public int Order { get; set; }
}
