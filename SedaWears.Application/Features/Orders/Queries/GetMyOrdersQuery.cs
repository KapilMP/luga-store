using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Orders.Queries;

public record OrderItemRepresentation(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public record OrderRepresentation(
    int Id,
    DateTime CreatedAt,
    string Status,
    decimal TotalAmount,
    List<OrderItemRepresentation> Items)
{
    public static OrderRepresentation ToOrderRepresentation(Order order) => new(
        order.Id,
        order.CreatedAt,
        order.Status.ToString(),
        order.TotalAmount,
        order.Items.Select(i => new OrderItemRepresentation(
            i.ProductId,
            i.Product?.Name ?? "Unknown",
            i.Quantity,
            i.UnitPrice,
            i.UnitPrice * i.Quantity)).ToList());
}

public record GetMyOrdersQuery() : IRequest<List<OrderRepresentation>>;

public record GetCustomerOrdersQuery(int userId) : IRequest<List<OrderRepresentation>>;

public class GetMyOrdersQueryHandler(IApplicationDbContext context, ICurrentUser currentUser) : IRequestHandler<GetMyOrdersQuery, List<OrderRepresentation>>
{
    public async Task<List<OrderRepresentation>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        return await GetOrdersInternal(userId, context, cancellationToken);
    }

    internal static async Task<List<OrderRepresentation>> GetOrdersInternal(int userId, IApplicationDbContext context, CancellationToken cancellationToken)
    {
        var orders = await context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(OrderRepresentation.ToOrderRepresentation).ToList();
    }
}

public class GetCustomerOrdersQueryHandler(IApplicationDbContext context) : IRequestHandler<GetCustomerOrdersQuery, List<OrderRepresentation>>
{
    public async Task<List<OrderRepresentation>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        return await GetMyOrdersQueryHandler.GetOrdersInternal(request.userId, context, cancellationToken);
    }
}
