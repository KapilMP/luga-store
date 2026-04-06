using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Application.Features.UserManagement.Queries;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Features.Orders.Commands;
using LugaStore.Application.Features.Orders.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class CustomersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<CustomerRepresentation>>> GetCustomers(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetCustomersQuery(pageNumber, pageSize, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerRepresentation>> GetCustomer(int id)
    {
        return await mediator.Send(new GetCustomerQuery(id));
    }

    [HttpGet("{customerId:int}/orders")]
    public async Task<ActionResult<List<LugaStore.Application.Features.Orders.OrderResponseDto>>> GetCustomerOrders(int customerId)
    {
        return await mediator.Send(new GetMyOrdersQuery(customerId));
    }

    [HttpPatch("{customerId:int}/orders/{orderId:int}/status")]
    public async Task<ActionResult> UpdateOrderStatus(int customerId, int orderId, [FromBody] OrderStatus status)
    {
        await mediator.Send(new UpdateOrderStatusCommand(orderId, customerId, status));
        return Ok();
    }
}
