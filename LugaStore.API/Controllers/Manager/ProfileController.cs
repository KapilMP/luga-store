using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Application.Features.Profile.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Manager;

[ApiController]
[Route("partner/{partnerId:int}/manager/[controller]")]
[Authorize(Roles = Roles.PartnerManager)]
public class ProfileController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PartnerManagerRepresentation>> GetProfile()
    {
        return await mediator.Send(new GetPartnerManagerProfileQuery(GetUserId()));
    }

    [HttpPatch]
    public async Task<ActionResult<PartnerManagerRepresentation>> UpdateProfile(UpdateProfileRequest request)
    {
        return await mediator.Send(new UpdatePartnerManagerProfileCommand(GetUserId(), request.FirstName, request.LastName, request.Phone));
    }

    [HttpGet("avatar/upload-url")]
    public async Task<ActionResult<ImageUploadUrlResponse>> GetAvatarUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
    {
        return await mediator.Send(new GetPartnerManagerAvatarUploadUrlCommand(GetUserId(), fileName, contentType));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand(GetUserId()));
        return NoContent();
    }
}
