using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Orders.Queries;

public record GetMyOrdersQuery(int UserId) : IRequest<List<Order>>;

public class GetMyOrdersQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyOrdersQuery, List<Order>>
{
    public async Task<List<Order>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Created)
            .ToListAsync(cancellationToken);
    }
}
