using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Orders.Commands;
using LugaStore.Application.Features.Orders.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class OrdersController(ISender mediator) : ControllerBase
{
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetCustomerOrders(int customerId)
        => Ok(await mediator.Send(new GetCustomerOrdersQuery(customerId)));

    [HttpPatch("{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] int customerId, [FromBody] OrderStatus status)
    {
        await mediator.Send(new UpdateOrderStatusCommand(orderId, customerId, status));
        return Ok();
    }
}
