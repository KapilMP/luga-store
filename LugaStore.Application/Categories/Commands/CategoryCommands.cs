using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Application.Categories;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Categories.Commands;

public record CreateCategoryCommand(string Name, string Slug, string? Description, int? PartnerId = null) : IRequest<int>;
public record UpdateCategoryCommand(int Id, string Name, string Slug, string? Description, int? PartnerId = null) : IRequest<Unit>;
public record DeleteCategoryCommand(int Id, int? PartnerId = null) : IRequest<Unit>;
public record ReorderCategoriesCommand(List<CategoryOrderDto> Orders, int? PartnerId = null) : IRequest<Unit>;
public record CategoryOrderDto(int Id, int DisplayOrder);

public class CategoryCommandHandlers(IApplicationDbContext dbContext) : 
    IRequestHandler<CreateCategoryCommand, int>,
    IRequestHandler<UpdateCategoryCommand, Unit>,
    IRequestHandler<DeleteCategoryCommand, Unit>,
    IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var finalOrder = (await dbContext.Categories
            .Where(c => c.PartnerId == request.PartnerId)
            .MaxAsync(c => (int?)c.DisplayOrder, cancellationToken) ?? 0) + 1;

        var category = new Category
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            DisplayOrder = finalOrder,
            PartnerId = request.PartnerId
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], cancellationToken);
        if (category == null) throw new NotFoundError("Category not found");

        if (request.PartnerId.HasValue && category.PartnerId != request.PartnerId)
            throw new ForbiddenError("Unauthorized access to category");

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], cancellationToken);
        if (category == null) throw new NotFoundError("Category not found");

        if (request.PartnerId.HasValue && category.PartnerId != request.PartnerId)
            throw new ForbiddenError("Unauthorized access to category");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Orders.Select(o => o.Id).ToList();
        var query = dbContext.Categories.Where(c => ids.Contains(c.Id));
        
        if (request.PartnerId.HasValue)
            query = query.Where(c => c.PartnerId == request.PartnerId);

        var categories = await query.ToListAsync(cancellationToken);

        foreach (var orderDto in request.Orders)
        {
            var category = categories.FirstOrDefault(c => c.Id == orderDto.Id);
            if (category != null)
                category.DisplayOrder = orderDto.DisplayOrder;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
