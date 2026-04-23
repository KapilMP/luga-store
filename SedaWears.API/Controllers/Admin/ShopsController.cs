using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Application.Features.Users.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Admin;

public record CreateShopRequest(string Name, string Slug, string? Description);
public record InviteRequest(string Email);

[ApiController]
[Route("admin/shops")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ShopsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShops(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopsQuery(pageNumber, pageSize, sortBy, sortOrder)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShop(int id)
        => Ok(await mediator.Send(new GetShopQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateShopRequest request)
    {
        await mediator.Send(new CreateShopCommand(request.Name, request.Slug, request.Description));
        return Ok(new { message = "Shop created successfully." });
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteShopCommand(id));
        return Ok(new { message = "Shop deleted successfully." });
    }

    [HttpPost("{shopId:int}/owners/invite")]
    public async Task<IActionResult> InviteOwner(int shopId, [FromBody] InviteRequest request)
    {
        await mediator.Send(new AdminInviteShopOwnerCommand(shopId, request.Email));
        return Ok(new { message = "Invitation sent successfully." });
    }

    [HttpPost("{shopId:int}/managers/invite")]
    public async Task<IActionResult> InviteManager(int shopId, [FromBody] InviteRequest request)
    {
        await mediator.Send(new AdminInviteShopManagerCommand(shopId, request.Email));
        return Ok(new { message = "Invitation sent successfully." });
    }
}

