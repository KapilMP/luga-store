using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Categories;
using LugaStore.Application.Categories.Commands;
using LugaStore.Application.Categories.Queries;
using MediatR;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Admin;



[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await mediator.Send(new GetCategoriesQuery(), ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryUpsertRequest request, CancellationToken ct)
    {
        return Ok(await mediator.Send(new CreateCategoryCommand(request.Name, request.Description), ct));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Description), ct);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id), ct);
        return Ok();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder(CategoryReorderRequest request, CancellationToken ct)
    {
        var orders = request.Orders.Select(o => new CategoryOrderDto(o.Id, o.DisplayOrder)).ToList();
        await mediator.Send(new ReorderCategoriesCommand(orders, PartnerId: null), ct);
        return Ok();
    }
}
