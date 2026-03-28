using LugaStore.Domain.Enums;

namespace LugaStore.Application.Common.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(int userId, int productId, ProductSize size, int quantity, CancellationToken ct = default);
    Task<bool> UpdateCartItemAsync(int itemId, int userId, ProductSize size, int quantity, CancellationToken ct = default);
    Task<bool> RemoveCartItemAsync(int itemId, int userId, CancellationToken ct = default);
    Task ClearCartAsync(int userId, CancellationToken ct = default);
}
