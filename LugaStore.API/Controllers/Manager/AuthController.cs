using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Features.Auth.Commands;
using LugaStore.Application.Features.Invitations.Commands;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.UserManagement.Commands;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;

namespace LugaStore.API.Controllers.Manager;

public record LoginRequest(string Email, string Password);

[Route("manager/[controller]")]
[EnableRateLimiting(nameof(LugaStore.Application.Common.Settings.RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOptions<RefreshTokenPathsConfig> options,
    IOptions<JwtConfig> jwtOptions,
    ITokenService tokenService,
    IWebHostEnvironment environment) : LugaStoreControllerBase
{
    private string AuthRefreshPath => options.Value.PartnerManagerRefreshPath; 
    private SameSiteMode CookieSameSite => environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict;
    private bool CookieSecure => !environment.IsDevelopment();

    [HttpPost("login")]
    public async Task<ActionResult<AdminAuthResponse<PartnerManagerRepresentation>>> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new PartnerManagerLoginCommand(request.Email, request.Password));
        SetAuthCookies(refreshToken, AuthRefreshPath);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(AuthRefreshPath);
        return NoContent();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AdminAuthResponse<PartnerManagerRepresentation>>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var csrfTokenCookie = Request.Cookies["csrfToken"];
        var csrfHeader = Request.Headers["X-CSRF-TOKEN"].ToString();

        if (string.IsNullOrEmpty(refreshToken) ||
            string.IsNullOrEmpty(csrfTokenCookie) ||
            csrfHeader != csrfTokenCookie)
            return Unauthorized("Invalid refresh request (CSRF failure).");

        var result = await mediator.Send(new PartnerManagerRefreshTokenCommand(refreshToken));
        if (result == null) return Unauthorized("Refresh session expired.");

        var (accessToken, newRefreshToken, user) = result.Value;
        SetAuthCookies(newRefreshToken, AuthRefreshPath);
        return Ok(new AdminAuthResponse<PartnerManagerRepresentation>(accessToken, user));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Current password is incorrect.");
        return Ok("Password changed successfully.");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("If the user exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or user session.");
        return Ok("Password has been reset.");
    }

    private void SetAuthCookies(string refreshToken, string refreshPath)
    {
        var csrf = tokenService.GenerateCsrfToken();
        var sameSite = CookieSameSite;
        var secure = CookieSecure;

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = sameSite,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        });
        Response.Cookies.Append("csrfToken", csrf, new CookieOptions
        {
            HttpOnly = false,
            Secure = secure,
            SameSite = sameSite,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        });
    }

    private void ClearAuthCookies(string refreshPath)
    {
        var sameSite = CookieSameSite;
        var secure = CookieSecure;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = sameSite,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Delete("refreshToken", cookieOptions);

        var csrfCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = secure,
            SameSite = sameSite,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(-1)
        };
        Response.Cookies.Delete("csrfToken", csrfCookieOptions);
    }
}
