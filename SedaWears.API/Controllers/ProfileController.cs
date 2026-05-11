using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Profile.Queries;
using SedaWears.Application.Features.Shops.Queries;
using SedaWears.Domain.Enums;

using SedaWears.Application.Features.Users.Commands;
using SedaWears.Application.Features.Users.Models;

namespace SedaWears.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
        => Ok(await mediator.Send(new GetMeQuery()));

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordRequest request)
    {
        await mediator.Send(new ChangeUserPasswordCommand(request.NewPassword));
        return NoContent();
    }

    [HttpGet("shops")]
    [Authorize(Roles = $"{nameof(UserRole.Owner)},{nameof(UserRole.Manager)}")]
    public async Task<IActionResult> GetMyShops(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] string? search = null)
        => Ok(await mediator.Send(new GetMyShopsQuery(pageNumber, pageSize, sortBy, sortOrder, search)));
}
