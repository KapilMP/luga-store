using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Categories;

public record CategoryReorderRequest(List<CategoryOrderItemRequest> Orders);
public record CategoryOrderItemRequest(int Id, int DisplayOrder);
public record CategoryOrderDto(int Id, int DisplayOrder);

public record ReorderCategoriesCommand(List<CategoryOrderDto> Orders, int? ShopId = null) : IRequest<Unit>;

public class ReorderCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken ct)
    {
        var ids = request.Orders.Select(o => o.Id).ToList();
        var query = dbContext.Categories.Where(c => ids.Contains(c.Id));
        if (request.ShopId.HasValue) query = query.Where(c => c.ShopId == request.ShopId);
        var categories = await query.ToListAsync(ct);

        foreach (var orderDto in request.Orders)
        {
            var category = categories.FirstOrDefault(c => c.Id == orderDto.Id);
            if (category != null) category.DisplayOrder = orderDto.DisplayOrder;
        }

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
