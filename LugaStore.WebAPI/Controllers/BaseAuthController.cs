using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Infrastructure.Settings;
using LugaStore.Application.Identity.Commands;

namespace LugaStore.WebAPI.Controllers;

public abstract class BaseAuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : LugaStoreControllerBase
{
    protected ISender Mediator => mediator;
    protected IRefreshTokenPaths RefreshTokenPaths => cookieSettings;
    protected abstract string AuthRefreshPath { get; }

    [HttpPost("logout")]
    public virtual IActionResult Logout()
    {
        ClearAuthCookies(AuthRefreshPath);
        return NoContent();
    }

    [HttpPost("refresh")]
    public virtual async Task<ActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var refreshCsrfCookie = Request.Cookies["refreshCsrf"];
        var csrfHeader = Request.Headers["C-CSRF-TOKEN"].ToString();

        if (string.IsNullOrEmpty(refreshToken) ||
            string.IsNullOrEmpty(refreshCsrfCookie) ||
            csrfHeader != refreshCsrfCookie)
            return Unauthorized("Invalid refresh request (CSRF failure).");

        var result = await mediator.Send(new RefreshTokenCommand(refreshToken));
        if (result == null) return Unauthorized("Refresh session expired.");

        SetAuthCookies(result.Value.RefreshToken, AuthRefreshPath);
        return Ok(new { accessToken = result.Value.AccessToken });
    }

    [HttpPost("change-password")]
    public virtual async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Current password is incorrect.");
        return Ok("Password changed successfully.");
    }

    [HttpPost("forgot-password")]
    public virtual async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("If the user exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public virtual async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or user session.");
        return Ok("Password has been reset.");
    }

    protected void SetAuthCookies(string refreshToken, string refreshPath)
    {
        var csrf = Guid.NewGuid().ToString();
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        Response.Cookies.Append("refreshCsrf", csrf, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    protected void ClearAuthCookies(string refreshPath)
    {
        var opts = new CookieOptions { Secure = true, SameSite = SameSiteMode.Strict, Path = refreshPath, Expires = DateTime.UtcNow.AddDays(-1) };
        Response.Cookies.Append("refreshToken", "", new CookieOptions { HttpOnly = true, Secure = opts.Secure, SameSite = opts.SameSite, Path = opts.Path, Expires = opts.Expires });
        Response.Cookies.Append("refreshCsrf", "", new CookieOptions { HttpOnly = false, Secure = opts.Secure, SameSite = opts.SameSite, Path = opts.Path, Expires = opts.Expires });
    }
}
