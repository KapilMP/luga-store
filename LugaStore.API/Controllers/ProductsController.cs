using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Products.Commands;
using LugaStore.Application.Features.Products.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;

namespace LugaStore.API.Controllers;

public record ProductUpsertRequest(string Name, string? Description, decimal Price, List<ProductSizeUpsertRequest> Sizes);
public record ProductSizeUpsertRequest(ProductSize Size, int Stock);

[ApiController]
[Route("[controller]")]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null, [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetProductsQuery(pageNumber, pageSize, categoryId, search)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await mediator.Send(new GetProductQuery(id)));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(ProductUpsertRequest request)
        => Ok(await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Sizes.Select(s => new ProductSizeCommandDto(s.Size, s.Stock)).ToList())));

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, ProductUpsertRequest request)
    {
        await mediator.Send(new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.Sizes.Select(s => new ProductSizeCommandDto(s.Size, s.Stock)).ToList()));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
