using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Cart;
using LugaStore.Application.Cart.Commands;
using LugaStore.Application.Cart.Queries;

namespace LugaStore.WebAPI.Controllers.Customer;

[Route("customer/[controller]")]
[Authorize]
public class CartController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart()
        => Ok(await mediator.Send(new GetCartQuery(CurrentUserId)));

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        await mediator.Send(new AddToCartCommand(CurrentUserId, request.ProductId, request.Size, request.Quantity));
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, UpdateCartRequest request)
    {
        var result = await mediator.Send(new UpdateCartItemCommand(id, CurrentUserId, request.Size, request.Quantity));
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var result = await mediator.Send(new RemoveCartItemCommand(id, CurrentUserId));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await mediator.Send(new ClearCartCommand(CurrentUserId));
        return NoContent();
    }
}
