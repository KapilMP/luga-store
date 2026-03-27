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
public class PartnerManagersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPartnerManagers()
    {
        var result = await mediator.Send(new GetPartnerManagersQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPartnerManager(int id)
    {
        var result = await mediator.Send(new GetPartnerManagerQuery(id));
        return Ok(result);
    }

    [HttpPost("resend-invitation")]
    public async Task<IActionResult> ResendInvitation(ResendInvitationCommand command)
    {
        await mediator.Send(command);
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivatePartnerManager(int id)
    {
        await mediator.Send(new ActivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivatePartnerManager(int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePartnerManager(int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(id));
        return NoContent();
    }
}
