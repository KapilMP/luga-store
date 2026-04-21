using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SedaWears.Application.Features.Profile.Queries;

namespace SedaWears.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
        => Ok(await mediator.Send(new GetMeQuery()));
}
