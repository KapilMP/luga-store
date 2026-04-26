using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Application.Features.Invitations.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Shop;

public record UpdateShopOwnerRequest(string FirstName, string LastName, bool IsActive);

[ApiController]
[Route("shops/{shopId:int}/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class OwnersController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> InviteShopOwner(int shopId, [FromBody] ShopMemberInviteRequest request)
    {
        await mediator.Send(new InviteShopMemberCommand(shopId, request.Email, UserRole.Owner));
        return Ok(new { message = "Owner added to shop successfully." });
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> GetShopOwners(
        int shopId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] bool? isInvited = null)
        => Ok(await mediator.Send(new GetShopMembersQuery(shopId, UserRole.Owner, pageNumber, pageSize, sortBy, sortOrder, isInvited)));

    [HttpGet("{ownerId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> GetShopOwner([FromRoute] int shopId, [FromRoute] int ownerId)
        => Ok(await mediator.Send(new GetShopMemberQuery(shopId, ownerId)));

    [HttpPatch("{ownerId:int}")]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<IActionResult> UpdateShopOwner([FromRoute] int shopId, [FromRoute] int ownerId, [FromBody] UpdateShopOwnerRequest request)
    {
        await mediator.Send(new UpdateShopMemberCommand(
            shopId,
            ownerId,
            request.FirstName,
            request.LastName,
            request.IsActive));
        return Ok(new { message = "Owner updated successfully." });
    }

    [HttpDelete("{ownerId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteShopOwner([FromRoute] int shopId, [FromRoute] int ownerId)
    {
        await mediator.Send(new DeleteShopMemberCommand(shopId, ownerId));
        return Ok(new { message = "Owner deleted successfully." });
    }

    [HttpPost("{ownerId:int}/resend-invitation")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> ResendShopOwnerInvitation(int shopId, int ownerId)
    {
        await mediator.Send(new ResendShopMemberInvitationCommand(shopId, ownerId, UserRole.Owner));
        return Ok(new { message = "Shop owner invitation resent successfully." });
    }

    [HttpPatch("{ownerId:int}/activate")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> ActivateShopOwner(int shopId, int ownerId)
    {
        await mediator.Send(new SetShopMemberActiveStatusCommand(shopId, ownerId, true));
        return Ok(new { message = "Owner activated successfully." });
    }

    [HttpPatch("{ownerId:int}/deactivate")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> DeactivateShopOwner(int shopId, int ownerId)
    {
        await mediator.Send(new SetShopMemberActiveStatusCommand(shopId, ownerId, false));
        return Ok(new { message = "Owner deactivated successfully." });
    }
}
