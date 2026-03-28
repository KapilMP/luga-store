using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.Application.Identity;

namespace LugaStore.WebAPI.Controllers.Manager;

[Route("manager/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(mediator, cookieSettings)
{
    protected override string AuthRefreshPath => RefreshTokenPaths.PartnerRefreshPath; // Using partner refresh path for managers

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await Mediator.Send(new PartnerManagerLoginCommand(request.Email, request.Password));
        SetAuthCookies(result.RefreshToken, AuthRefreshPath);
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }
}
