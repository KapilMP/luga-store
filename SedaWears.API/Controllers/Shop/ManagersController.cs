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

public record UpdateShopManagerRequest(string FirstName, string LastName, bool IsActive);
public record UpdateShopManagerActiveStatusRequest(bool IsActive);

[ApiController]
[Route("shops/{shopId:int}/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ManagersController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> InviteShopManager(int shopId, [FromBody] ShopMemberInviteRequest request)
    {
        await mediator.Send(new InviteShopMemberCommand(shopId, request.Email, UserRole.Manager));
        return Ok(new { message = "Manager invited successfully." });
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> GetShopManagers(
        int shopId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isInvited = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopMembersQuery(shopId, UserRole.Manager, pageNumber, pageSize, sortBy, sortOrder, isInvited)));

    [HttpGet("{managerId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Manager)}")]
    public async Task<IActionResult> GetShopManager([FromRoute] int shopId, [FromRoute] int managerId)
        => Ok(await mediator.Send(new GetShopMemberQuery(shopId, managerId)));

    [HttpPatch("{managerId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Manager)}")]
    public async Task<IActionResult> UpdateShopManager([FromRoute] int shopId, [FromRoute] int managerId, [FromBody] UpdateShopManagerRequest request)
    {
        await mediator.Send(new UpdateShopMemberCommand(
            shopId,
            managerId,
            request.FirstName,
            request.LastName,
            request.IsActive));
        return Ok(new { message = "Manager updated successfully." });
    }

    [HttpDelete("{managerId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> DeleteShopManager([FromRoute] int shopId, [FromRoute] int managerId)
    {
        await mediator.Send(new DeleteShopMemberCommand(shopId, managerId));
        return Ok(new { message = "Manager deleted successfully." });
    }

    [HttpPost("{managerId:int}/resend-invitation")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> ResendShopManagerInvitation(int shopId, int managerId)
    {
        await mediator.Send(new ResendShopMemberInvitationCommand(shopId, managerId, UserRole.Manager));
        return Ok(new { message = "Shop manager invitation resent successfully." });
    }

    [HttpPatch("{managerId:int}/status")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
    public async Task<IActionResult> UpdateShopManagerActiveStatus(int shopId, int managerId, [FromBody] UpdateShopManagerActiveStatusRequest request)
    {
        await mediator.Send(new UpdateShopMemberActiveStatusCommand(shopId, managerId, request.IsActive));
        return Ok(new { message = $"Shop manager {(request.IsActive ? "activated" : "deactivated")} successfully." });
    }
}
