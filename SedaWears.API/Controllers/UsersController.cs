using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Users.Queries;
using SedaWears.Application.Features.Users.Commands;
using SedaWears.Application.Features.Invitations.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers;

public record UpdateUserRequest(string FirstName, string LastName, bool? IsActive, string? NewPassword);
public record InviteAdminRequest(string Email);
public record UpdateAdminActiveStatusRequest(bool IsActive);

[ApiController]
[Route("")]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet("admins")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAdmins(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isInvited = false,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetAdminsQuery(pageNumber, pageSize, isInvited, sortBy, sortOrder)));

    [HttpPost("admins")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> InviteAdmin([FromBody] InviteAdminRequest request)
    {
        await mediator.Send(new InviteAdminCommand(request.Email));
        return Ok(new { Message = "Invitation sent successfully." });
    }

    [HttpPatch("admins/{id:int}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateAdminActiveStatus(int id, [FromBody] UpdateAdminActiveStatusRequest request)
    {
        await mediator.Send(new UpdateUserActiveStatusCommand(id, request.IsActive, UserRole.Admin));
        return NoContent();
    }

    [HttpGet("customers")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isInvited = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetCustomersQuery(pageNumber, pageSize, isInvited, sortBy, sortOrder)));


    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(int id)
        => Ok(await mediator.Send(new GetUserQuery(Id: id)));


    [HttpPatch("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        => Ok(await mediator.Send(new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.IsActive,
            request.NewPassword)));

    [HttpDelete("admins/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        await mediator.Send(new DeleteAdminCommand(id));
        return NoContent();
    }

    [HttpDelete("customers/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await mediator.Send(new DeleteCustomerCommand(id));
        return NoContent();
    }

    [HttpPost("admins/{id:int}/resend-invitation")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ResendAdminInvitation(int id)
    {
        await mediator.Send(new ResendAdminInvitationCommand(id));
        return NoContent();
    }


}

