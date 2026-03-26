using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.PartnerManager;

public record CreateUserRequest(string Email, string FirstName, string LastName);

[ApiController]
[Route("partner/{partnerId:int}/manager/[controller]")]
[Authorize(Roles = Roles.PartnerManager + "," + Roles.Admin)]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpPost("partner")]
    public async Task<IActionResult> CreatePartner(CreateUserRequest request)
    {
        if (!await mediator.Send(new CreatePartnerCommand(request.Email, request.FirstName, request.LastName)))
            return Conflict("Email already exists.");
        return Ok("Invitation sent.");
    }

    [HttpPatch("{id:int}/active-status")]
    public async Task<IActionResult> SetPartnerActiveStatus(int id, [FromBody] bool isActive)
    {
        if (!await mediator.Send(new SetUserActiveStatusCommand(id, isActive)))
            return NotFound();
        return Ok();
    }
}
