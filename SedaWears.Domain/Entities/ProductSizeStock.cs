using SedaWears.Domain.Common;
using SedaWears.Domain.Enums;

namespace SedaWears.Domain.Entities;

public class ProductSizeStock : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public ProductSize Size { get; set; }
    public int Stock { get; set; }
}
