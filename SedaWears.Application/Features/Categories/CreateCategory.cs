using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Categories;

public record CategoryUpsertRequest(string Name, string Slug, string? Description);

public record CreateCategoryCommand(string Name, string Slug, string? Description, int? ShopId = null) : IRequest<int>;

public class CreateCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var finalOrder = (await dbContext.Categories
            .Where(c => c.ShopId == request.ShopId)
            .MaxAsync(c => (int?)c.DisplayOrder, ct) ?? 0) + 1;

        var category = new Category { Name = request.Name, Slug = request.Slug, Description = request.Description, DisplayOrder = finalOrder, ShopId = request.ShopId };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(ct);
        return category.Id;
    }
}
