using LugaStore.Application.Categories;

namespace LugaStore.Application.Common.Interfaces;

public interface ICategoryService
{
    // For  Admin
    Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CreateAsync(string name, string? description, CancellationToken cancellationToken);
    Task UpdateAsync(int id, string name, string? description, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task ReorderAsync(List<(int id, int order)> orders, CancellationToken cancellationToken);

    // For Partner
    Task<List<CategoryDto>> GetPartnerCategoriesAsync(int partnerId, CancellationToken cancellationToken);
    Task<int> CreatePartnerCategoryAsync(int partnerId, string name, string? description, CancellationToken cancellationToken);
    Task UpdatePartnerCategoryAsync(int partnerId, int id, string name, string? description, CancellationToken cancellationToken);
    Task DeletePartnerCategoryAsync(int partnerId, int id, CancellationToken cancellationToken);
    Task ReorderPartnerCategoriesAsync(int partnerId, List<(int id, int order)> orders, CancellationToken cancellationToken);
}
