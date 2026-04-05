using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Orders;

namespace LugaStore.Application.Orders.Queries;

public record GetMyOrdersQuery(int UserId) : IRequest<List<OrderResponseDto>>;

public class GetMyOrdersQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyOrdersQuery, List<OrderResponseDto>>
{
    public async Task<List<OrderResponseDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Created)
            .ToListAsync(cancellationToken);
            
        return orders.Select(o => new OrderResponseDto(
            o.Id,
            o.Status.ToString(),
            o.TotalAmount,
            o.Created,
            o.Items.Select(i => new OrderItemResponseDto(
                i.ProductId,
                i.Product.Name,
                i.Quantity,
                i.UnitPrice,
                i.UnitPrice * i.Quantity))
        )).ToList();
    }
}
