using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Controllers.Manager;

public record CreateProductRequest(string Name, string? Description, decimal Price, ProductCategory Category);
public record SetSizesRequest(List<ProductSizeStockDto> Sizes);

[ApiController]
[Route("manager/[controller]")]
[Authorize(Roles = Roles.PartnerManager + "," + Roles.Admin)]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStoreProducts()
        => Ok(await mediator.Send(new GetAllProductsQuery()));

    [HttpPost]
    public async Task<IActionResult> ManageProduct(CreateProductRequest request)
    {
        var id = await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Category));
        return CreatedAtAction(nameof(GetStoreProducts), new { id }, id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOperationalProduct(int id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id, IsAdmin: true));
        if (!result) return NotFound();
        return NoContent();
    }
}
