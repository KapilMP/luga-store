using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ManagerController(ISender mediator) : ControllerBase
{
    [HttpPost("invite")]
    public async Task<IActionResult> InviteManager(InvitePartnerManagerCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return Conflict("Email already exists.");
        return Ok("Invitation sent.");
    }

    [HttpPost("resend-invitation")]
    public async Task<IActionResult> ResendInvitation(ResendInvitationCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivateManager(int id)
    {
        await mediator.Send(new ActivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateManager(int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(id));
        return NoContent();
    }
}
