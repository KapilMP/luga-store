using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Categories;
using LugaStore.Domain.Common;

using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetCategoriesQuery(), ct));

    [HttpGet("partner/{partnerId:int}")]
    [Authorize(Roles = Roles.PartnerManager)]
    public async Task<IActionResult> GetPartnerCategories(int partnerId, CancellationToken ct)
        => Ok(await mediator.Send(new GetCategoriesQuery(partnerId), ct));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(CategoryUpsertRequest request, CancellationToken ct)
        => Ok(await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description), ct));

    [HttpPost("partner/{partnerId:int}")]
    [Authorize(Roles = Roles.PartnerManager)]
    public async Task<IActionResult> CreatePartnerCategory(int partnerId, CategoryUpsertRequest request, CancellationToken ct)
        => Ok(await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description, partnerId), ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Slug, request.Description), ct);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id), ct);
        return Ok();
    }

    [HttpPost("reorder")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Reorder(CategoryReorderRequest request, CancellationToken ct)
    {
        var orders = request.Orders.Select(o => new CategoryOrderDto(o.Id, o.DisplayOrder)).ToList();
        await mediator.Send(new ReorderCategoriesCommand(orders), ct);
        return Ok();
    }
}
