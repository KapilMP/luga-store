using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.Auth.Commands;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOptions<RefreshTokenPathsConfig> options,
    IOptions<JwtConfig> jwtOptions,
    IWebHostEnvironment environment) : ControllerBase
{
    private string RefreshPath => options.Value.CustomerRefreshPath;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new LoginCommand(request.Email, request.Password, Roles.Customer));
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName, request.Phone));
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token");

        var (response, newRefreshToken) = await mediator.Send(new RefreshTokenCommand(refreshToken, Roles.Customer));
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
