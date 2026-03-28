using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.Application.Identity;

namespace LugaStore.WebAPI.Controllers.Admin;

[Route("admin/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(mediator, cookieSettings)
{
    protected override string AuthRefreshPath => RefreshTokenPaths.AdminRefreshPath;

    [HttpPost("login")]
    public async Task<ActionResult> Login(AdminLoginCommand command)
    {
        var result = await Mediator.Send(command);
        SetAuthCookies(result.RefreshToken, AuthRefreshPath);
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

}
