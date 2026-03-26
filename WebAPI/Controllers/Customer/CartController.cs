using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Cart.Commands;
using LugaStore.Application.Cart.Queries;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Controllers.Customer;

public record AddToCartRequest(int ProductId, ProductSize Size, int Quantity);
public record UpdateCartRequest(ProductSize Size, int Quantity);

[ApiController]
[Route("customer/[controller]")]
[Authorize]
public class CartController(ISender mediator, IUserService userService) : ControllerBase
{
    private int CurrentUserId => int.Parse(userService.UserId!);

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
