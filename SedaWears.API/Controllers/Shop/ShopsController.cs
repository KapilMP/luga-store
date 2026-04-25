using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Application.Features.Shops.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Shop;

public record UpsertShopRequest(string Name, string Slug, string? Description, bool IsActive, string? LogoFileName, string? BannerFileName);
public record ShopMemberInviteRequest(string Email);

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ShopsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetShops(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetShopsQuery(pageNumber, pageSize, sortBy, sortOrder, search)));

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetShop(int id)
        => Ok(await mediator.Send(new GetShopQuery(id)));

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
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
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)},{nameof(UserRole.Manager)}")]
    public async Task<IActionResult> UpdateShop(int id, UpsertShopRequest request)
    {
        await mediator.Send(new UpdateShopCommand(id, request.Name, request.Slug, request.Description, request.IsActive, request.LogoFileName, request.BannerFileName));
        return Ok(new { message = "Shop updated successfully." });
    }

    [HttpPatch("{id:int}/activate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ActivateShop(int id)
    {
        await mediator.Send(new SetShopActiveStatusCommand(id, true));
        return Ok(new { message = "Shop activated successfully." });
    }

    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeactivateShop(int id)
    {
        await mediator.Send(new SetShopActiveStatusCommand(id, false));
        return Ok(new { message = "Shop deactivated successfully." });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteShop(int id)
    {
        await mediator.Send(new DeleteShopCommand(id));
        return Ok(new { message = "Shop deleted successfully." });
    }
}
