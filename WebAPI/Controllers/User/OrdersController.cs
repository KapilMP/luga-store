using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.WebAPI.Controllers.User;

public record CreateOrderRequest(
    string? CustomerName, 
    string? CustomerEmail, 
    string? ShippingAddress,
    List<OrderItemRequest> Items);

public record OrderItemRequest(int ProductId, int Quantity);

[ApiController]
[Route("user/[controller]")]
public class OrdersController(
    IApplicationDbContext context,
    IUserService userService) : ControllerBase
{
    [HttpPost("checkout")]
    [AllowAnonymous] // Support Guest Checkout
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest request)
    {
        var currentUserIdString = userService.UserId;
        int? userId = string.IsNullOrEmpty(currentUserIdString) ? null : int.Parse(currentUserIdString);

        // Guest Validation
        if (userId == null && (string.IsNullOrEmpty(request.CustomerEmail) || string.IsNullOrEmpty(request.CustomerName)))
        {
            return BadRequest("Guest checkout requires Name and Email.");
        }

        var order = new Order
        {
            UserId = userId,
            CustomerName = request.CustomerName ?? string.Empty,
            CustomerEmail = request.CustomerEmail ?? string.Empty,
            ShippingAddress = request.ShippingAddress ?? string.Empty,
            Status = OrderStatus.Pending,
            TotalAmount = 0
        };

        foreach (var item in request.Items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null) return BadRequest($"Product {item.ProductId} not found.");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

            order.TotalAmount += product.Price * item.Quantity;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(default);

        return Ok(new { orderId = order.Id, status = order.Status, total = order.TotalAmount });
    }

    [HttpGet("my-history")]
    [Authorize] // History requires login
    public async Task<IActionResult> GetMyHistory()
    {
        var currentUserId = int.Parse(userService.UserId!);
        
        var orders = await context.Orders
            .Where(o => o.UserId == currentUserId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Created)
            .ToListAsync();

        return Ok(orders);
    }
}
