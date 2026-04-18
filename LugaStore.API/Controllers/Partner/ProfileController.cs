using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Application.Features.Profile.Models;
using LugaStore.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetPartnerProfileQuery());
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await mediator.Send(new UpdatePartnerProfileCommand(request.FirstName, request.LastName, request.Phone, request.AvatarFileName));
        return Ok(result);
    }

    [HttpGet("avatar/upload-url")]
    public async Task<IActionResult> GetAvatarUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
    {
        var result = await mediator.Send(new GetPartnerAvatarUploadUrlCommand(fileName, contentType));
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand());
        return NoContent();
    }
}
