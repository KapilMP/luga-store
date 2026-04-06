using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Products.Commands;
using LugaStore.Application.Features.Products.Queries;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

using LugaStore.Domain.Enums;

using LugaStore.Application.Features.Products;

namespace LugaStore.API.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProductsController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet("my-creations")]
    public async Task<IActionResult> GetMyProducts()
        => Ok(await mediator.Send(new GetMyProductsQuery(GetUserId())));

    [HttpPost]
    public async Task<IActionResult> CreateAsPartner(ProductUpsertRequest request)
    {
        var id = await mediator.Send(new CreatePartnerProductCommand(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds, GetUserId()));
        return CreatedAtAction(nameof(GetMyProducts), new { id }, id);
    }

    [HttpPatch("{id:int}/sizes")]
    public async Task<IActionResult> SetSizes(int id, SetSizesRequest request)
    {
        await mediator.Send(new SetProductSizesCommand(id, request.Sizes, RequestingUserId: GetUserId()));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOwnedProduct(int id)
    {
        await mediator.Send(new DeleteProductCommand(id, RequestingUserId: GetUserId()));
        return NoContent();
    }
}
