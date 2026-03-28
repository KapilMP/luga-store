using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Queries;
using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseAll(
        [FromQuery] Gender? gender,
        [FromQuery] int? categoryId,
        [FromQuery] string? categorySlug,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var products = await mediator.Send(new GetProductsQuery(gender, categoryId, categorySlug, sortBy, pageNumber, pageSize));
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        if (product == null) return NotFound();
        return Ok(product);
    }
}
