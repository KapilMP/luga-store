using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Products.Queries;

namespace LugaStore.WebAPI.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseAll()
    {
        var products = await mediator.Send(new GetProductsQuery());
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
