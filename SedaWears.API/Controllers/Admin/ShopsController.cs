using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Application.Features.Users.Commands;
using SedaWears.Application.Features.Users.Queries;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Admin;

public record UpsertShopRequest(string Name, string Slug, string? Description, bool IsActive, string? LogoFileName, string? BannerFileName);
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
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopsQuery(pageNumber, pageSize, sortBy, sortOrder)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShop(int id)
        => Ok(await mediator.Send(new GetShopQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create(UpsertShopRequest request)
    {
        await mediator.Send(new CreateShopCommand(
            request.Name,
            request.Slug,
            request.Description,
            request.IsActive,
            request.LogoFileName,
            request.BannerFileName));
        return Ok(new { message = "Shop created successfully." });
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertShopRequest request)
    {
        await mediator.Send(new UpdateShopCommand(id, request.Name, request.Slug, request.Description, request.IsActive, request.LogoFileName, request.BannerFileName));
        return Ok(new { message = "Shop updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteShopCommand(id));
        return Ok(new { message = "Shop deleted successfully." });
    }

    [HttpPost("{shopId:int}/owners")]
    public async Task<IActionResult> InviteOwner(int shopId, [FromBody] InviteRequest request)
    {
        await mediator.Send(new AdminInviteShopOwnerCommand(shopId, request.Email));
        return Ok(new { message = "Invitation sent successfully." });
    }

    [HttpGet("{shopId:int}/owners")]
    public async Task<IActionResult> GetShopOwners(
        int shopId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopOwnersQuery(shopId, pageNumber, pageSize, sortBy, sortOrder)));

    [HttpPost("{shopId:int}/managers")]
    public async Task<IActionResult> InviteManager(int shopId, [FromBody] InviteRequest request)
    {
        await mediator.Send(new AdminInviteShopManagerCommand(shopId, request.Email));
        return Ok(new { message = "Invitation sent successfully." });
    }

    [HttpGet("{shopId:int}/managers")]
    public async Task<IActionResult> GetShopManagers(
        int shopId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopManagersByShopIdQuery(shopId, pageNumber, pageSize, null, sortBy, sortOrder)));

    [HttpDelete("{shopId:int}/managers/{managerId:int}")]
    public async Task<IActionResult> DeleteShopManager(int shopId, int managerId)
    {
        await mediator.Send(new DeleteManagerCommand(managerId));
        return Ok(new { message = "Manager deleted successfully." });
    }

    [HttpDelete("{shopId:int}/owners/{ownerId:int}")]
    public async Task<IActionResult> DeleteShopOwner(int shopId, int ownerId)
    {
        await mediator.Send(new DeleteOwnerCommand(ownerId));
        return Ok(new { message = "Owner deleted successfully." });
    }

    [HttpPost("{shopId:int}/owners/{ownerId:int}/resend-invitation")]
    public async Task<IActionResult> ResendOwnerInvitation(int shopId, int ownerId)
    {
        await mediator.Send(new ResendInvitationCommand(ownerId, UserRole.Owner));
        return Ok(new { message = "Invitation resent successfully." });
    }

    [HttpPost("{shopId:int}/managers/{managerId:int}/resend-invitation")]
    public async Task<IActionResult> ResendManagerInvitation(int shopId, int managerId)
    {
        await mediator.Send(new ResendInvitationCommand(managerId, UserRole.Manager));
        return Ok(new { message = "Invitation resent successfully." });
    }

    [HttpPatch("{shopId:int}/owners/{ownerId:int}/activate")]
    public async Task<IActionResult> ActivateShopOwner(int shopId, int ownerId)
    {
        await mediator.Send(new SetUserActiveStatusCommand(ownerId, true));
        return Ok(new { message = "Owner activated successfully." });
    }

    [HttpPatch("{shopId:int}/managers/{managerId:int}/activate")]
    public async Task<IActionResult> ActivateShopManager(int shopId, int managerId)
    {
        await mediator.Send(new SetUserActiveStatusCommand(managerId, true));
        return Ok(new { message = "Manager activated successfully." });
    }

    [HttpPatch("{shopId:int}/owners/{ownerId:int}/deactivate")]
    public async Task<IActionResult> DeactivateShopOwner(int shopId, int ownerId)
    {
        await mediator.Send(new SetUserActiveStatusCommand(ownerId, false));
        return Ok(new { message = "Owner deactivated successfully." });
    }

    [HttpPatch("{shopId:int}/managers/{managerId:int}/deactivate")]
    public async Task<IActionResult> DeactivateShopManager(int shopId, int managerId)
    {
        await mediator.Send(new SetUserActiveStatusCommand(managerId, false));
        return Ok(new { message = "Manager deactivated successfully." });
    }
}

