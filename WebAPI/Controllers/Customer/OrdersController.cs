using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Orders.Commands;
using LugaStore.Application.Orders.Queries;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.WebAPI.Controllers.Customer;

public record CheckoutAddressRequest(string FullName, string Phone, string Street, string City, string ZipCode);
public record CreateOrderRequest(string? CustomerEmail, CheckoutAddressRequest? ShippingAddress, List<OrderItemRequest> Items);
public record OrderItemRequest(int ProductId, int Quantity);

[ApiController]
[Route("customer/[controller]")]
public class OrdersController(ISender mediator, IUserService userService) : ControllerBase
{
    [HttpPost("checkout")]
    [AllowAnonymous]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest request)
    {
        var userIdString = userService.UserId;
        int? userId = string.IsNullOrEmpty(userIdString) ? null : int.Parse(userIdString);

        var address = request.ShippingAddress is { } a
            ? new CheckoutAddressDto(a.FullName, a.Phone, a.Street, a.City, a.ZipCode)
            : null;

        var items = request.Items.Select(i => new CheckoutItemDto(i.ProductId, i.Quantity)).ToList();

        var result = await mediator.Send(new CheckoutCommand(userId, request.CustomerEmail, address, items));
        return Ok(new { orderId = result.OrderId, status = result.Status, total = result.Total });
    }

    [HttpGet("my-history")]
    [Authorize]
    public async Task<IActionResult> GetMyHistory()
    {
        var userId = int.Parse(userService.UserId!);
        var orders = await mediator.Send(new GetMyOrdersQuery(userId));
        return Ok(orders);
    }
}
