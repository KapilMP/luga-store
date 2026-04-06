using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Categories.Queries;
using MediatR;

namespace LugaStore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await mediator.Send(new GetCategoriesQuery(PartnerId: null), ct));
    }
}
