using SedaWears.Application.Features.Users.Queries;
using SedaWears.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers.Owner;

public record InviteManagerRequest(string Email);

[ApiController]
[Route("owner/[controller]")]
[Authorize(Roles = nameof(UserRole.Owner))]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ManagerController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ManagerRepresentation>>> GetManagers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetMyManagersQuery(pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ManagerRepresentation>> GetManager(int id)
    {
        return await mediator.Send(new GetManagerQuery(id));
    }

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InviteManager(InviteManagerRequest request)
    {
        await mediator.Send(new InviteManagerCommand(request.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<ActionResult<string>> ResendInvitation(int id)
    {
        await mediator.Send(new ResendInvitationCommand(id, UserRole.Manager));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult> ActivateManager(int id)
    {
        await mediator.Send(new ActivateManagerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivateManager(int id)
    {
        await mediator.Send(new DeactivateManagerCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeleteManagerCommand(id));
        return NoContent();
    }
}
