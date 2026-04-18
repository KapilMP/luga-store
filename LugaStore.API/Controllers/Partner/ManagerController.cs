using LugaStore.Application.Features.Users.Queries;
using LugaStore.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers.Partner;

public record InviteManagerRequest(string Email);

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ManagerController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<PartnerManagerRepresentation>>> GetManagers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetPartnerManagersQuery(pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartnerManagerRepresentation>> GetManager(int id)
    {
        return await mediator.Send(new GetPartnerManagerQuery(id));
    }

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InviteManager(InviteManagerRequest request)
    {
        await mediator.Send(new InvitePartnerManagerCommand(request.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<ActionResult<string>> ResendInvitation(int id)
    {
        await mediator.Send(new ResendPartnerManagerInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult> ActivateManager(int id)
    {
        await mediator.Send(new ActivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivateManager(int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(id));
        return NoContent();
    }
}
