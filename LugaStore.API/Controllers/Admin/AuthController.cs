using Microsoft.Extensions.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Auth.Commands;
using LugaStore.Application.Auth.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Application.Common.Configurations;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.API.Controllers.Admin;

[Route("admin/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(
    ISender mediator,
    IOptions<RefreshTokenPathsConfig> options,
    IOptions<JwtConfig> jwtOptions,
    ITokenService tokenService,
    IWebHostEnvironment environment) : LugaStoreControllerBase
{
    private string AuthRefreshPath => options.Value.AdminRefreshPath;
    private SameSiteMode CookieSameSite => environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict;
    private bool CookieSecure => !environment.IsDevelopment();

    [HttpPost("login")]
    public async Task<ActionResult<AdminAuthResponse<AdminRepresentation>>> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new AdminLoginCommand(request.Email, request.Password));
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
    public async Task<ActionResult<AdminAuthResponse<AdminRepresentation>>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var csrfTokenCookie = Request.Cookies["csrfToken"];
        var csrfHeader = Request.Headers["X-CSRF-TOKEN"].ToString();

        if (string.IsNullOrEmpty(refreshToken) ||
            string.IsNullOrEmpty(csrfTokenCookie) ||
            csrfHeader != csrfTokenCookie)
            return Unauthorized("Invalid refresh request (CSRF failure).");

        var result = await mediator.Send(new AdminRefreshTokenCommand(refreshToken));
        if (result == null) return Unauthorized("Refresh session expired.");

        var (accessToken, newRefreshToken, user) = result.Value;
        SetAuthCookies(newRefreshToken, AuthRefreshPath);
        return Ok(new AdminAuthResponse<AdminRepresentation>(accessToken, user));
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
