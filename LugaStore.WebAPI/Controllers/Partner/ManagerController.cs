using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Identity.Queries;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ManagerController(ISender mediator, IPartnerService partnerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetManagers()
    {
        var managers = await partnerService.GetManagersAsync();
        return Ok(managers);
    }

    [HttpGet("invited")]
    public async Task<IActionResult> GetInvitedManagers()
    {
        var result = await mediator.Send(new GetInvitedManagersQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetManager(int id)
    {
        var manager = await partnerService.GetManagerByManagerIdAsync(id);
        return Ok(manager);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteManager(InviteManagerCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<IActionResult> ResendInvitation(int id)
    {
        await mediator.Send(new ResendManagerInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivateManager(int id)
    {
        await mediator.Send(new ActivateManagerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateManager(int id)
    {
        await mediator.Send(new DeactivateManagerCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeleteManagerCommand(id));
        return NoContent();
    }
}
