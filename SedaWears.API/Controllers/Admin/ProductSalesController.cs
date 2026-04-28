using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Products.Commands;
using SedaWears.Application.Features.Products.Queries;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Admin;

public record ProductSaleUpsertRequest(decimal DiscountedPrice, decimal? DiscountPercent, DateTime StartsAt, DateTime? EndsAt, bool IsActive = true);

[ApiController]
[Route("admin/products/{productId:int}/sales")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ProductSalesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int productId)
        => Ok(await mediator.Send(new GetProductSalesQuery(productId)));

    [HttpPost]
    public async Task<IActionResult> Create(int productId, ProductSaleUpsertRequest request)
        => Ok(await mediator.Send(new CreateProductSaleCommand(productId, request.DiscountedPrice, request.DiscountPercent, request.StartsAt, request.EndsAt)));

    [HttpPut("{saleId:int}")]
    public async Task<IActionResult> Update(int productId, int saleId, ProductSaleUpsertRequest request)
    {
        await mediator.Send(new UpdateProductSaleCommand(productId, saleId, request.DiscountedPrice, request.DiscountPercent, request.StartsAt, request.EndsAt, request.IsActive));
        return Ok();
    }

    [HttpDelete("{saleId:int}")]
    public async Task<IActionResult> Delete(int productId, int saleId)
    {
        await mediator.Send(new DeleteProductSaleCommand(productId, saleId));
        return NoContent();
    }
}
