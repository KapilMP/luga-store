using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Profile.Commands;
using LugaStore.Application.Profile.Queries;
using LugaStore.Application.Profile.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class ProfileController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AdminRepresentation>> GetProfile()
    {
        return await mediator.Send(new GetAdminProfileQuery(GetUserId()));
    }

    [HttpPatch]
    public async Task<ActionResult<AdminRepresentation>> UpdateProfile(UpdateProfileRequest request)
    {
        return await mediator.Send(new UpdateAdminProfileCommand(GetUserId(), request.FirstName, request.LastName, request.Phone));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand(GetUserId()));
        return NoContent();
    }
}
