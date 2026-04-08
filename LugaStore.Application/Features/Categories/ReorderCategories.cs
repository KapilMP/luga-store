using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Categories;

public record CategoryReorderRequest(List<CategoryOrderItemRequest> Orders);
public record CategoryOrderItemRequest(int Id, int DisplayOrder);
public record CategoryOrderDto(int Id, int DisplayOrder);

public record ReorderCategoriesCommand(List<CategoryOrderDto> Orders, int? PartnerId = null) : IRequest<Unit>;

public class ReorderCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken ct)
    {
        var ids = request.Orders.Select(o => o.Id).ToList();
        var query = dbContext.Categories.Where(c => ids.Contains(c.Id));
        if (request.PartnerId.HasValue) query = query.Where(c => c.PartnerId == request.PartnerId);
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
