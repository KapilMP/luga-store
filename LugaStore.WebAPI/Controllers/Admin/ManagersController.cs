using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Identity.Queries;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Admin;

[ApiController]
[Route("admin/partner/{partnerId:int}/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ManagersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPartnerManagers(int partnerId)
    {
        var result = await mediator.Send(new GetPartnerManagersQuery(partnerId));
        return Ok(result);
    }

    [HttpGet("invited")]
    public async Task<IActionResult> GetInvitedPartnerManagers(int partnerId)
    {
        var result = await mediator.Send(new GetInvitedPartnerManagersQuery(partnerId));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPartnerManager(int partnerId, int id)
    {
        var result = await mediator.Send(new GetPartnerManagerQuery(partnerId, id));
        return Ok(result);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InvitePartnerManager(int partnerId, InviteManagerCommand command)
    {
        await mediator.Send(new InvitePartnerManagerCommand(partnerId, command.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<IActionResult> ResendInvitation(int partnerId, int id)
    {
        await mediator.Send(new ResendPartnerManagerInvitationCommand(partnerId, id));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivatePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new ActivatePartnerManagerCommand(partnerId, id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivatePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(partnerId, id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(partnerId, id));
        return NoContent();
    }
}
