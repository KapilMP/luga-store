using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Admin;

public record CreateUserRequest(string Email, string FirstName, string LastName);
public record AcceptInvitationRequest(string Email, string Token, string Password);

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController(ISender mediator, IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
        => Ok(new { userId = userService.UserId, role = userService.Role });

    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdmin(CreateUserRequest request)
    {
        if (!await mediator.Send(new CreateAdminCommand(request.Email, request.FirstName, request.LastName)))
            return Conflict("Email already exists.");
        return Ok("Invitation sent.");
    }

    [HttpPost("partner")]
    public async Task<IActionResult> CreatePartner(CreateUserRequest request)
    {
        if (!await mediator.Send(new CreatePartnerCommand(request.Email, request.FirstName, request.LastName)))
            return Conflict("Email already exists.");
        return Ok("Invitation sent.");
    }

    [HttpPost("partner-manager")]
    public async Task<IActionResult> CreatePartnerManager(CreateUserRequest request)
    {
        if (!await mediator.Send(new CreatePartnerManagerCommand(request.Email, request.FirstName, request.LastName)))
            return Conflict("Email already exists.");
        return Ok("Invitation sent.");
    }

    [HttpPost("accept-invitation")]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptInvitation(AcceptInvitationRequest request)
    {
        if (!await mediator.Send(new AcceptInvitationCommand(request.Email, request.Token, request.Password)))
            return BadRequest("Invalid or expired invitation.");
        return Ok("Account activated. You can now log in.");
    }

    [HttpPatch("{id:int}/active-status")]
    public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
    {
        if (!await mediator.Send(new SetUserActiveStatusCommand(id, isActive)))
            return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (!await mediator.Send(new DeleteUserCommand(id)))
            return NotFound();
        return NoContent();
    }
}
