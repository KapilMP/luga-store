using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Categories.Queries;

namespace LugaStore.WebAPI.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }
}
