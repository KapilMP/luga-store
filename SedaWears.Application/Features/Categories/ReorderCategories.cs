using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

using SedaWears.Application.Features.Categories.Models;

namespace SedaWears.Application.Features.Categories;

public record ReorderCategoryItem(int Id, int DisplayOrder);

public record ReorderCategoriesCommand(List<ReorderCategoryItem> Orders, int? ShopId = null) : IRequest<Unit>;

public class ReorderCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken ct)
    {
        var ids = request.Orders.Select(o => o.Id).ToList();
        var query = dbContext.Categories.Where(c => ids.Contains(c.Id));
        if (request.ShopId.HasValue) query = query.Where(c => c.ShopId == request.ShopId);
        var categories = await query.ToListAsync(ct);

        foreach (var order in request.Orders)
        {
            var category = categories.FirstOrDefault(c => c.Id == order.Id);
            if (category != null) category.DisplayOrder = order.DisplayOrder;
        }

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
