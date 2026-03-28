using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

using LugaStore.Domain.Enums;

using LugaStore.Application.Products;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProductsController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet("my-creations")]
    public async Task<IActionResult> GetMyProducts()
        => Ok(await mediator.Send(new GetMyProductsQuery(CurrentUserId)));

    [HttpPost]
    public async Task<IActionResult> CreateAsPartner(ProductUpsertRequest request)
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
