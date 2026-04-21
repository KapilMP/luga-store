using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Admin;

public record CreateShopRequest(string Name, string Slug, string? Description);

[ApiController]
[Route("admin/shops")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ShopsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShops(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopsQuery(pageNumber, pageSize, isActive, sortBy, sortOrder)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateShopRequest request)
    {
        await mediator.Send(new CreateShopCommand(request.Name, request.Slug, request.Description));
        return Ok(new { message = "Shop created successfully." });
    }
}
