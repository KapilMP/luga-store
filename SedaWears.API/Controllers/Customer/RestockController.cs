using MediatR;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Products.Commands;

namespace SedaWears.API.Controllers.Customer;

public record SubscribeRestockRequest(string Email, string Size);

[ApiController]
[Route("products/{productId:int}/restock")]
public class RestockController(ISender mediator) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(int productId, SubscribeRestockRequest request)
    {
        await mediator.Send(new SubscribeRestockCommand(productId, request.Email, request.Size));
        return Ok("You will be notified when this item is back in stock.");
    }
}
