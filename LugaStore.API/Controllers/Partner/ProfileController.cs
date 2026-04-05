using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Profile.Commands;
using LugaStore.Application.Profile.Queries;
using LugaStore.Application.Profile.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProfileController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetPartnerProfileQuery(GetUserId()));
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await mediator.Send(new UpdatePartnerProfileCommand(GetUserId(), request.FirstName, request.LastName, request.Phone));
        return Ok(result);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadPartnerAvatarCommand(GetUserId(), stream, file.FileName));
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand(GetUserId()));
        return NoContent();
    }
}
