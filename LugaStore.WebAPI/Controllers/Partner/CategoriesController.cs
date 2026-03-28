using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Categories;
using LugaStore.Application.Categories.Commands;
using LugaStore.Application.Categories.Queries;
using MediatR;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Partner;



[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class CategoriesController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await mediator.Send(new GetCategoriesQuery(PartnerId: CurrentUserId), ct);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryUpsertRequest request, CancellationToken ct)
    {
        var category = await mediator.Send(new CreateCategoryCommand(request.Name, request.Description, PartnerId: CurrentUserId), ct);
        return Ok(category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Description, PartnerId: CurrentUserId), ct);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id, PartnerId: CurrentUserId), ct);
        return Ok();
    }
}
