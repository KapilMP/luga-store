using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Application.Features.Auth.Commands;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Common;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOriginContext originContext,
    IOptions<JwtConfig> jwtOptions,
    IWebHostEnvironment environment) : ControllerBase
{
    private string RefreshPath => originContext.RefreshPath;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new LoginCommand(request.Email, request.Password));
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (response, refreshToken) = await mediator.Send(
            new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone)
        );
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token");

        var (response, newRefreshToken) = await mediator.Send(new RefreshTokenCommand(refreshToken));
        SetAuthCookies(newRefreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(originContext.RefreshPath);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> Forgot(ForgotPasswordRequest req)
    {
        await mediator.Send(new ForgotPasswordCommand(req.Email));
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset(ResetPasswordRequest req)
    {
        await mediator.Send(new ResetPasswordCommand(req.Email, req.Token, req.NewPassword));
        return Ok();
    }

    private void SetAuthCookies(string token, string path)
    {
        var secure = environment.IsProduction();
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Strict,
            Path = path,
            Expires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        });
    }

    private void ClearAuthCookies(string path)
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = path });
    }
}
