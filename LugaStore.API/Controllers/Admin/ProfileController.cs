using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Application.Features.Profile.Models;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AdminRepresentation>> GetProfile()
    {
        return await mediator.Send(new GetAdminProfileQuery());
    }

    [HttpPatch]
    public async Task<ActionResult<AdminRepresentation>> UpdateProfile(UpdateProfileRequest request)
    {
        return await mediator.Send(new UpdateAdminProfileCommand(request.FirstName, request.LastName, request.Phone, request.AvatarFileName));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand());
        return NoContent();
    }
}
