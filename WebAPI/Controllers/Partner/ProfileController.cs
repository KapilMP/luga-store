using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Identity.Queries;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetProfileQuery<PartnerProfileDto>());
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand<PartnerProfileDto> command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadAvatarCommand<PartnerProfileDto>(stream, file.FileName));
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand());
        return NoContent();
    }
}
