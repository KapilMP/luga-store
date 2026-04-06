using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Categories.Queries;
using LugaStore.Application.Common.Settings;
using Microsoft.AspNetCore.RateLimiting;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Search))]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }
}
