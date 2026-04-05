using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Profile.Commands;
using LugaStore.Application.Profile.Queries;
using LugaStore.Application.Profile.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[Authorize(Roles = Roles.Customer)]
public class ProfileController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CustomerRepresentation>> GetProfile()
    {
        return await mediator.Send(new GetCustomerProfileQuery(GetUserId()));
    }

    [HttpPatch]
    public async Task<ActionResult<CustomerRepresentation>> UpdateProfile(UpdateProfileRequest request)
    {
        return await mediator.Send(new UpdateCustomerProfileCommand(GetUserId(), request.FirstName, request.LastName, request.Phone));
    }

    [HttpPut("avatar")]
    public async Task<ActionResult<CustomerRepresentation>> UploadAvatar(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return await mediator.Send(new UploadCustomerAvatarCommand(GetUserId(), stream, file.FileName));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand(GetUserId()));
        return NoContent();
    }
}
