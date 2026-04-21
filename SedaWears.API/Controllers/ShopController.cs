using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Shops.Models;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<PaginatedList<ShopRepresentation>>> GetShops([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isActive = null)
        => Ok(await mediator.Send(new GetShopsQuery(pageNumber, pageSize, isActive)));

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ShopRepresentation>> GetShop(int id)
        => Ok(await mediator.Send(new GetShopQuery(id)));

    [HttpPatch("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Owner))]
    public async Task<ActionResult> Update(int id, UpdateShopRequest request)
    {
        await mediator.Send(new UpdateShopCommand(id, request.Name, request.Slug, request.Description, request.IsActive));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteShopCommand(id));
        return NoContent();
    }
}

public record UpdateShopRequest(string Name, string Slug, string? Description, bool? IsActive);
