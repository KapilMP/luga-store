using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Controllers.Partner;

public record CreatePartnerProductRequest(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds);
public record SetSizesRequest(List<ProductSizeStockDto> Sizes);

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProductsController(ISender mediator, ICurrentUser currentUser) : ControllerBase
{
    private int CurrentUserId => int.Parse(currentUser.UserId!);

    [HttpGet("my-creations")]
    public async Task<IActionResult> GetMyProducts()
        => Ok(await mediator.Send(new GetMyProductsQuery(CurrentUserId)));

    [HttpPost]
    public async Task<IActionResult> CreateAsPartner(CreatePartnerProductRequest request)
    {
        var id = await mediator.Send(new CreatePartnerProductCommand(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds, CurrentUserId));
        return CreatedAtAction(nameof(GetMyProducts), new { id }, id);
    }

    [HttpPost("{id:int}/sizes")]
    public async Task<IActionResult> SetSizes(int id, SetSizesRequest request)
    {
        var result = await mediator.Send(new SetProductSizesCommand(id, request.Sizes, RequestingUserId: CurrentUserId));
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOwnedProduct(int id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id, RequestingUserId: CurrentUserId));
        if (!result) return NotFound();
        return NoContent();
    }
}
