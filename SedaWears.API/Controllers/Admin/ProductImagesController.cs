using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Products.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Admin;

public record AddProductImageRequest(string FileName);
public record ReorderProductImagesRequest(List<ReorderProductImageItem> Items);

[ApiController]
[Route("admin/products/{productId:int}/images")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ProductImagesController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add(int productId, AddProductImageRequest request)
        => Ok(await mediator.Send(new AddProductImageCommand(productId, request.FileName)));

    [HttpDelete("{imageId:int}")]
    public async Task<IActionResult> Delete(int productId, int imageId)
    {
        await mediator.Send(new DeleteProductImageCommand(productId, imageId));
        return NoContent();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder(int productId, ReorderProductImagesRequest request)
    {
        await mediator.Send(new ReorderProductImagesCommand(productId, request.Items));
        return Ok();
    }
}
