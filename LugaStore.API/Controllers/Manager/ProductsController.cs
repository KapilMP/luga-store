using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Commands;
using LugaStore.Application.Products.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

using LugaStore.Application.Products;

namespace LugaStore.API.Controllers.Manager;

[ApiController]
[Route("manager/[controller]")]
[Authorize(Roles = Roles.PartnerManager + "," + Roles.Admin)]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStoreProducts()
        => Ok(await mediator.Send(new GetAllProductsQuery()));

    [HttpPost]
    public async Task<IActionResult> ManageProduct(ProductUpsertRequest request)
    {
        var id = await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds));
        return CreatedAtAction(nameof(GetStoreProducts), new { id }, id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOperationalProduct(int id)
    {
        await mediator.Send(new DeleteProductCommand(id, IsAdmin: true));
        return NoContent();
    }
}
