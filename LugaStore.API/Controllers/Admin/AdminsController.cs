using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Invitations.Commands;
using LugaStore.Application.Features.UserManagement.Commands;
using LugaStore.Application.Features.UserManagement.Queries;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<AdminRepresentation>>> GetAdmins(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetAdminsQuery(pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminRepresentation>> GetAdmin(int id)
    {
        return await mediator.Send(new GetAdminQuery(id));
    }

    public record InviteAdminRequest(string Email);

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InviteAdmin(InviteAdminRequest request)
    {
        await mediator.Send(new InviteAdminCommand(request.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<ActionResult<string>> ResendInvitation(int id)
    {
        await mediator.Send(new ResendAdminInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult> ActivateAdmin(int id)
    {
        await mediator.Send(new ActivateAdminCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivateAdmin(int id)
    {
        await mediator.Send(new DeactivateAdminCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAdmin(int id)
    {
        await mediator.Send(new DeleteAdminCommand(id));
        return NoContent();
    }
}
