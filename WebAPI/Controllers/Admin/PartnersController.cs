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
public class PartnersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPartners()
    {
        var result = await mediator.Send(new GetPartnersQuery());
        return Ok(result);
    }

    [HttpGet("invited")]
    public async Task<IActionResult> GetInvitedPartners()
    {
        var result = await mediator.Send(new GetInvitedPartnersQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPartner(int id)
    {
        var result = await mediator.Send(new GetPartnerQuery(id));
        return Ok(result);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InvitePartner(InvitePartnerCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<IActionResult> ResendInvitation(int id)
    {
        await mediator.Send(new ResendPartnerInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePartner(int id)
    {
        await mediator.Send(new DeletePartnerCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivatePartner(int id)
    {
        await mediator.Send(new ActivatePartnerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivatePartner(int id)
    {
        await mediator.Send(new DeactivatePartnerCommand(id));
        return Ok();
    }
}
