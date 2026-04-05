using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Cart;
using LugaStore.Application.Cart.Commands;
using LugaStore.Application.Cart.Queries;

namespace LugaStore.API.Controllers.Customer;

[Route("customer/[controller]")]
[Authorize]
public class CartController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart()
        => Ok(await mediator.Send(new GetCartQuery(GetUserId())));

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        await mediator.Send(new AddToCartCommand(GetUserId(), request.ProductId, request.Size, request.Quantity));
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, UpdateCartRequest request)
    {
        await mediator.Send(new UpdateCartItemCommand(id, GetUserId(), request.Size, request.Quantity));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        await mediator.Send(new RemoveCartItemCommand(id, GetUserId()));
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await mediator.Send(new ClearCartCommand(GetUserId()));
        return NoContent();
    }
}
