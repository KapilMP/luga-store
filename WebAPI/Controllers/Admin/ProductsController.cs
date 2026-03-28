using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Controllers.Admin;

public record CreateProductRequest(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds);
public record SetSizesRequest(List<ProductSizeStockDto> Sizes);

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
        => Ok(await mediator.Send(new GetAllProductsQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        var id = await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds));
        return CreatedAtAction(nameof(GetAllProducts), new { id }, id);
    }

    [HttpPost("{id:int}/sizes")]
    public async Task<IActionResult> SetSizes(int id, SetSizesRequest request)
    {
        var result = await mediator.Send(new SetProductSizesCommand(id, request.Sizes, IsAdmin: true));
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id, IsAdmin: true));
        if (!result) return NotFound();
        return NoContent();
    }


}
