using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Invitations.Commands;
using LugaStore.Application.UserManagement.Commands;
using LugaStore.Application.UserManagement.Queries;
using LugaStore.Application.Common.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/partner/{partnerId:int}/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ManagersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<PartnerManagerRepresentation>>> GetPartnerManagers(
        int partnerId, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetPartnerManagersQuery(partnerId, pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartnerManagerRepresentation>> GetPartnerManager(int partnerId, int id)
    {
        return await mediator.Send(new GetPartnerManagerQuery(partnerId, id));
    }

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InvitePartnerManager(int partnerId, InviteManagerRequest request)
    {
        await mediator.Send(new InvitePartnerManagerCommand(partnerId, request.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<ActionResult<string>> ResendInvitation(int partnerId, int id)
    {
        await mediator.Send(new ResendPartnerManagerInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult> ActivatePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new ActivatePartnerManagerCommand(partnerId, id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivatePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(partnerId, id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePartnerManager(int partnerId, int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(partnerId, id));
        return NoContent();
    }
}
