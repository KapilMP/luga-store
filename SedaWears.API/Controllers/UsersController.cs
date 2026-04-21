using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Users.Queries;
using SedaWears.Application.Features.Users.Commands;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("")]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet("admins")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetAdmins(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? isActive = null, 
        [FromQuery] bool? isInvited = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetAdminsQuery(pageNumber, pageSize, isActive, isInvited, sortBy, sortOrder)));

    [HttpPost("admins/invite")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> InviteAdmin([FromBody] InviteAdminRequest request)
    {
        await mediator.Send(new InviteAdminCommand(request.Email));
        return NoContent();
    }

    [HttpPost("owners/invite")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> InviteOwner([FromBody] InviteOwnerRequest request)
    {
        await mediator.Send(new InviteOwnerCommand(request.Email));
        return NoContent();
    }

    [HttpPost("shops/{shopId:int}/managers/invite")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> AdminInviteShopManager([FromRoute] int shopId, [FromBody] InviteShopManagerRequest request)
    {
        await mediator.Send(new AdminInviteShopManagerCommand(shopId, request.Email));
        return NoContent();
    }

    [HttpPost("shop/managers/invite")]
    [Authorize(Roles = nameof(UserRole.Owner))]
    public async Task<IActionResult> InviteShopManager([FromBody] InviteShopManagerRequest request)
    {
        await mediator.Send(new InviteManagerCommand(request.Email));
        return NoContent();
    }

    [HttpGet("owners")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetOwners(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? isActive = null, 
        [FromQuery] bool? isInvited = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetOwnersQuery(pageNumber, pageSize, isActive, isInvited, sortBy, sortOrder)));

    [HttpGet("customers")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? isActive = null, 
        [FromQuery] bool? isInvited = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetCustomersQuery(pageNumber, pageSize, isActive, isInvited, sortBy, sortOrder)));

    [HttpGet("shops/{shopId:int}/managers")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetShopManagersByShopId(
        [FromRoute] int shopId, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? isActive = null, 
        [FromQuery] bool? invited = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "desc")
        => Ok(await mediator.Send(new GetShopManagersByShopIdQuery(shopId, pageNumber, pageSize, invited, isActive, sortBy, sortOrder)));

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

    [HttpDelete("owners/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteOwner(int id)
    {
        await mediator.Send(new DeleteOwnerCommand(id));
        return NoContent();
    }

    [HttpDelete("managers/{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteManager(int id)
    {
        await mediator.Send(new DeleteManagerCommand(id));
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
        await mediator.Send(new ResendInvitationCommand(id, UserRole.Admin));
        return NoContent();
    }

    [HttpPost("owners/{id:int}/resend-invitation")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ResendOwnerInvitation(int id)
    {
        await mediator.Send(new ResendInvitationCommand(id, UserRole.Owner));
        return NoContent();
    }

    [HttpPost("managers/{id:int}/resend-invitation")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ResendManagerInvitation(int id)
    {
        await mediator.Send(new ResendInvitationCommand(id, UserRole.Manager));
        return NoContent();
    }
}


public record UpdateUserRequest(string FirstName, string LastName, bool? IsActive, string? NewPassword);
public record InviteAdminRequest(string Email);
public record InviteOwnerRequest(string Email);
public record InviteShopManagerRequest(string Email);
