using LugaStore.Application.Features.Users.Queries;
using LugaStore.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Users;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet("{role}")]
    public async Task<IActionResult> GetUsers(string role, [FromQuery] int? partnerId = null)
        => Ok(await mediator.Send(new GetUsersQuery(role, partnerId)));

    [HttpGet("{role}/{id:int}")]
    public async Task<IActionResult> GetUser(string role, int id)
        => Ok(await mediator.Send(new GetUserQuery(id, role)));

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] bool isActive)
    {
        await mediator.Send(new SetUserActiveStatusCommand(id, isActive));
        return Ok();
    }

    [HttpDelete("{role}/{id:int}")]
    public async Task<IActionResult> Delete(string role, int id)
    {
        await mediator.Send(new DeleteUserCommand(id, role));
        return NoContent();
    }

    [HttpPost("partner/{partnerId:int}/invite-manager")]
    public async Task<IActionResult> InviteManager(int partnerId, [FromBody] string email)
    {
        await mediator.Send(new InvitePartnerManagerCommand(partnerId, email));
        return Ok("Invitation sent.");
    }
}
