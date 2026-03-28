using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Queries;
using LugaStore.Application.Orders.Commands;
using LugaStore.Application.Orders.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.WebAPI.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class CustomersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await mediator.Send(new GetCustomersQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var result = await mediator.Send(new GetCustomerQuery(id));
        return Ok(result);
    }

    [HttpGet("{customerId:int}/orders")]
    public async Task<IActionResult> GetCustomerOrders(int customerId)
    {
        var result = await mediator.Send(new GetMyOrdersQuery(customerId));
        return Ok(result);
    }

    [HttpPatch("{customerId:int}/orders/{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int customerId, int orderId, [FromBody] OrderStatus status)
    {
        await mediator.Send(new UpdateOrderStatusCommand(orderId, customerId, status));
        return Ok();
    }
}
