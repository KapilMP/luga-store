using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Categories;

public record CategoryUpsertRequest(string Name, string Slug, string? Description);

public record CreateCategoryCommand(string Name, string Slug, string? Description, int? PartnerId = null) : IRequest<int>;

public class CreateCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var finalOrder = (await dbContext.Categories
            .Where(c => c.PartnerId == request.PartnerId)
            .MaxAsync(c => (int?)c.DisplayOrder, ct) ?? 0) + 1;

        var category = new Category { Name = request.Name, Slug = request.Slug, Description = request.Description, DisplayOrder = finalOrder, PartnerId = request.PartnerId };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(ct);
        return category.Id;
    }
}
