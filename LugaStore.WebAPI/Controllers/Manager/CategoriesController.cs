using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Categories;
using LugaStore.Application.Categories.Commands;
using LugaStore.Application.Categories.Queries;
using MediatR;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Manager;


[ApiController]
[Route("partner/{partnerId:int}/[controller]")]
[Authorize(Roles = Roles.PartnerManager)]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int partnerId, CancellationToken ct)
    {
        var categories = await mediator.Send(new GetCategoriesQuery(partnerId), ct);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int partnerId, CategoryUpsertRequest request, CancellationToken ct)
    {
        var category = await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description, partnerId), ct);
        return Ok(category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int partnerId, int id, CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Slug, request.Description, partnerId), ct);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int partnerId, int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id, partnerId), ct);
        return Ok();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder(int partnerId, CategoryReorderRequest request, CancellationToken ct)
    {
        var orders = request.Orders.Select(o => new CategoryOrderDto(o.Id, o.DisplayOrder)).ToList();
        await mediator.Send(new ReorderCategoriesCommand(orders, partnerId), ct);
        return Ok();
    }
}
