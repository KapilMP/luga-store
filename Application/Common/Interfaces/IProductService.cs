using LugaStore.Domain.Enums;
using LugaStore.Application.Products.Commands;

namespace LugaStore.Application.Common.Interfaces;

public interface IProductService
{
    Task<int> CreateProductAsync(string name, string? description, decimal price, Gender gender, List<int> categoryIds, int? partnerId = null, CancellationToken ct = default);
    Task<bool> SetProductSizesAsync(int productId, List<ProductSizeStockDto> sizes, int? requestingUserId = null, bool isAdmin = false, CancellationToken ct = default);
    Task<bool> DeleteProductAsync(int productId, int? requestingUserId = null, bool isAdmin = false, CancellationToken ct = default);
}
