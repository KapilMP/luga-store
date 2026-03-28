using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Categories;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Services;

public class CategoryService(IApplicationDbContext dbContext) : ICategoryService
{
    // For Admin
    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await GetCategoriesAsync(null, cancellationToken);
    }

    public async Task<int> CreateAsync(string name, string? description, CancellationToken cancellationToken)
    {
        return await CreateInternalAsync(name, description, null, cancellationToken);
    }

    public async Task UpdateAsync(int id, string name, string? description, CancellationToken cancellationToken)
    {
        await UpdateInternalAsync(id, name, description, null, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await DeleteInternalAsync(id, null, cancellationToken);
    }

    public async Task ReorderAsync(List<(int id, int order)> orders, CancellationToken cancellationToken)
    {
        await ReorderInternalAsync(orders, null, cancellationToken);
    }

    // For Partner
    public async Task<List<CategoryDto>> GetPartnerCategoriesAsync(int partnerId, CancellationToken cancellationToken)
    {
        return await GetCategoriesAsync(partnerId, cancellationToken);
    }

    public async Task<int> CreatePartnerCategoryAsync(int partnerId, string name, string? description, CancellationToken cancellationToken)
    {
        return await CreateInternalAsync(name, description, partnerId, cancellationToken);
    }

    public async Task UpdatePartnerCategoryAsync(int partnerId, int id, string name, string? description, CancellationToken cancellationToken)
    {
        await UpdateInternalAsync(id, name, description, partnerId, cancellationToken);
    }

    public async Task DeletePartnerCategoryAsync(int partnerId, int id, CancellationToken cancellationToken)
    {
        await DeleteInternalAsync(id, partnerId, cancellationToken);
    }

    public async Task ReorderPartnerCategoriesAsync(int partnerId, List<(int id, int order)> orders, CancellationToken cancellationToken)
    {
        await ReorderInternalAsync(orders, partnerId, cancellationToken);
    }

    // Helper methods
    private async Task<List<CategoryDto>> GetCategoriesAsync(int? partnerId, CancellationToken cancellationToken)
    {
        var query = dbContext.Categories.AsNoTracking();

        if (partnerId.HasValue)
            query = query.Where(c => c.PartnerId == partnerId);
        else 
            query = query.Where(c => c.PartnerId == null);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    private async Task<int> CreateInternalAsync(string name, string? description, int? partnerId, CancellationToken cancellationToken)
    {
        var finalOrder = (await dbContext.Categories
            .Where(c => c.PartnerId == partnerId)
            .MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0) + 1;

        var category = new Category
        {
            Name = name,
            Description = description,
            DisplayOrder = finalOrder,
            PartnerId = partnerId
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }

    private async Task UpdateInternalAsync(int id, string name, string? description, int? partnerId, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category == null) throw new Exception("Category not found");

        if (partnerId.HasValue && category.PartnerId != partnerId)
            throw new Exception("Unauthorized access to category");

        category.Name = name;
        category.Description = description;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task DeleteInternalAsync(int id, int? partnerId, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category == null) throw new Exception("Category not found");

        if (partnerId.HasValue && category.PartnerId != partnerId)
            throw new Exception("Unauthorized access to category");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ReorderInternalAsync(List<(int id, int order)> orders, int? partnerId, CancellationToken cancellationToken)
    {
        var ids = orders.Select(o => o.id).ToList();
        var query = dbContext.Categories.Where(c => ids.Contains(c.Id));
        
        if (partnerId.HasValue)
            query = query.Where(c => c.PartnerId == partnerId);

        var categories = await query.ToListAsync(cancellationToken);

        foreach (var orderDto in orders)
        {
            var category = categories.FirstOrDefault(c => c.Id == orderDto.id);
            if (category != null)
            {
                category.DisplayOrder = orderDto.order;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
