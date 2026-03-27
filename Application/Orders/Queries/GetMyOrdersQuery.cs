using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Orders.Queries;

public record GetMyOrdersQuery(int UserId) : IRequest<List<Order>>;

public class GetMyOrdersQueryHandler(IOrderService orderService) : IRequestHandler<GetMyOrdersQuery, List<Order>>
{
    public async Task<List<Order>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        => await orderService.GetOrdersByUserAsync(request.UserId, cancellationToken);
}
