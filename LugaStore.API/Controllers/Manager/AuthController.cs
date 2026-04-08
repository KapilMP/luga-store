using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.Auth.Commands;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Manager;

[ApiController]
[Route("manager/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOptions<RefreshTokenPathsConfig> options,
    IOptions<JwtConfig> jwtOptions,
    IWebHostEnvironment environment) : ControllerBase
{
    private string RefreshPath => options.Value.PartnerManagerRefreshPath;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new LoginCommand(request.Email, request.Password, Roles.PartnerManager));
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token");

        var (response, newRefreshToken) = await mediator.Send(new RefreshTokenCommand(refreshToken, Roles.PartnerManager));
        SetAuthCookies(newRefreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout() { ClearAuthCookies(RefreshPath); return NoContent(); }

    private void SetAuthCookies(string token, string path)
    {
        var secure = environment.IsProduction();
        Response.Cookies.Append("refreshToken", token, new CookieOptions { HttpOnly = true, Secure = secure, SameSite = SameSiteMode.Strict, Path = path, Expires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays) });
    }

    private void ClearAuthCookies(string path)
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = path });
    }
}
