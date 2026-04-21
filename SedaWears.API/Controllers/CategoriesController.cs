using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Categories;
using SedaWears.Domain.Enums;

using Microsoft.AspNetCore.RateLimiting;
using SedaWears.Application.Common.Settings;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetCategoriesQuery(), ct));

    [HttpGet("shop/{shopId:int}")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<IActionResult> GetShopCategories(int shopId, CancellationToken ct)
        => Ok(await mediator.Send(new GetCategoriesQuery(shopId), ct));

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create(CategoryUpsertRequest request, CancellationToken ct)
        => Ok(await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description), ct));

    [HttpPost("shop/{shopId:int}")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<IActionResult> CreateShopCategory(int shopId, CategoryUpsertRequest request, CancellationToken ct)
        => Ok(await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description, shopId), ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update(int id, CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Slug, request.Description), ct);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id), ct);
        return Ok();
    }

    [HttpPost("reorder")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Reorder(CategoryReorderRequest request, CancellationToken ct)
    {
        var orders = request.Orders.Select(o => new CategoryOrderDto(o.Id, o.DisplayOrder)).ToList();
        await mediator.Send(new ReorderCategoriesCommand(orders), ct);
        return Ok();
    }
}
