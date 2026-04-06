using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Invitations.Commands;
using LugaStore.Application.Features.UserManagement.Commands;
using LugaStore.Application.Features.UserManagement.Queries;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ManagerController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<PartnerManagerRepresentation>>> GetManagers(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetPartnerManagersQuery(GetUserId(), pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartnerManagerRepresentation>> GetManager(int id)
    {
        return await mediator.Send(new GetPartnerManagerQuery(GetUserId(), id));
    }

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InviteManager(InviteManagerRequest request)
    {
        await mediator.Send(new InvitePartnerManagerCommand(GetUserId(), request.Email));
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
        await mediator.Send(new ActivatePartnerManagerCommand(GetUserId(), id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivateManager(int id)
    {
        await mediator.Send(new DeactivatePartnerManagerCommand(GetUserId(), id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeletePartnerManagerCommand(GetUserId(), id));
        return NoContent();
    }
}
