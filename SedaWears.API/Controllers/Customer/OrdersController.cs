using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Orders.Commands;
using SedaWears.Application.Features.Orders.Queries;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using Microsoft.AspNetCore.RateLimiting;

namespace SedaWears.API.Controllers.Customer;

public record CheckoutAddressRequest(string FullName, string Phone, string Street, string City, string ZipCode);
public record OrderItemRequestDto(int ProductId, int Quantity);
public record CreateOrderRequest(string? CustomerEmail, CheckoutAddressRequest? ShippingAddress, List<OrderItemRequestDto> Items);

[ApiController]
[Route("customer/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class OrdersController(ISender mediator) : ControllerBase
{
    [HttpPost("checkout")]
    [AllowAnonymous]
    [EnableRateLimiting(nameof(RateLimitingPolicies.Checkout))]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest request)
    {
        var address = request.ShippingAddress is { } a
            ? new CheckoutAddressDto(a.FullName, a.Phone, a.Street, a.City, a.ZipCode)
            : null;

        var items = request.Items.Select(i => new CheckoutItemDto(i.ProductId, i.Quantity)).ToList();

        var result = await mediator.Send(new CheckoutCommand(request.CustomerEmail, address, items));
        return Ok(new { orderId = result.OrderId, status = result.Status, total = result.Total });
    }

    [HttpGet("my-history")]
    [Authorize]
    public async Task<IActionResult> GetMyHistory()
    {
        var orders = await mediator.Send(new GetMyOrdersQuery());
        return Ok(orders);
    }
}
