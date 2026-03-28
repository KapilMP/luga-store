using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Identity.Queries;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Manager;

[ApiController]
[Route("partner/{partnerId:int}/manager/[controller]")]
[Authorize(Roles = Roles.PartnerManager)]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetProfileQuery<PartnerManagerProfileDto>());
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand<PartnerManagerProfileDto> command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadAvatarCommand<PartnerManagerProfileDto>(stream, file.FileName));
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand());
        return NoContent();
    }
}
