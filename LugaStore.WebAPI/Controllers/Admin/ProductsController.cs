using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

using LugaStore.Application.Products;

namespace LugaStore.WebAPI.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
        => Ok(await mediator.Send(new GetAllProductsQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductUpsertRequest request)
    {
        var id = await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds));
        return CreatedAtAction(nameof(GetAllProducts), new { id }, id);
    }

    [HttpPost("{id:int}/sizes")]
    public async Task<IActionResult> SetSizes(int id, SetSizesRequest request)
    {
        await mediator.Send(new SetProductSizesCommand(id, request.Sizes, IsAdmin: true));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await mediator.Send(new DeleteProductCommand(id, IsAdmin: true));
        return NoContent();
    }


}
