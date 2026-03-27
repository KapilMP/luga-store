using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Identity.Queries;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAdmins()
    {
        var result = await mediator.Send(new GetAdminsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAdmin(int id)
    {
        var result = await mediator.Send(new GetAdminQuery(id));
        return Ok(result);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteAdmin(InviteAdminCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation sent.");
    }

    [HttpPost("resend-invitation")]
    public async Task<IActionResult> ResendInvitation(ResendInvitationCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivateAdmin(int id)
    {
        await mediator.Send(new ActivateAdminCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateAdmin(int id)
    {
        await mediator.Send(new DeactivateAdminCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        await mediator.Send(new DeleteAdminCommand(id));
        return NoContent();
    }
}
