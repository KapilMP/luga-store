using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.WebAPI.Controllers.User;

public record GoogleLoginRequest(string IdToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record ConfirmEmailRequest(string UserId, string Token);

[ApiController]
[Route("user/[controller]")] 
public class AuthController(
    ISender mediator, 
    IAntiforgery antiforgery, 
    IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var authResult = await mediator.Send(command);
        if (authResult == null) return Unauthorized("Invalid credentials.");

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
        {
            var refreshCsrf = Guid.NewGuid().ToString();
            SetAuthCookies(authResult.RefreshToken, refreshCsrf);
        }

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
            return BadRequest("Google ID Token is required");

        var authResult = await authService.LoginWithGoogleAsync(request.IdToken);
        if (authResult == null)
            return Unauthorized("Invalid Google Token");

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
        {
            var refreshCsrf = Guid.NewGuid().ToString();
            SetAuthCookies(authResult.RefreshToken, refreshCsrf);
        }

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await authService.ForgotPasswordAsync(request.Email);
        return Ok("If the user exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        if (!result) return BadRequest("Invalid Token or User");
        return Ok("Password has been reset.");
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(ConfirmEmailRequest request)
    {
        var result = await authService.ConfirmEmailAsync(request.UserId, request.Token);
        if (!result) return BadRequest("Invalid Token or User");
        return Ok("Email has been verified.");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var refreshCsrfCookie = Request.Cookies["refreshCsrf"];
        var csrfHeader = Request.Headers["X-XSRF-TOKEN"].ToString();

        if (string.IsNullOrEmpty(refreshToken) || 
            string.IsNullOrEmpty(refreshCsrfCookie) || 
            csrfHeader != refreshCsrfCookie)
        {
            return Unauthorized("Invalid refresh request (CSRF failure)");
        }

        var result = await authService.RefreshTokenAsync(refreshToken);
        if (result == null)
            return Unauthorized("Refresh session expired");

        var newRefreshCsrf = Guid.NewGuid().ToString();
        SetAuthCookies(result.Value.RefreshToken, newRefreshCsrf);

        return Ok(new { accessToken = result.Value.AccessToken });
    }

    private void SetAuthCookies(string refreshToken, string refreshCsrf)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Path = "/user/auth/refresh", 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var csrfOptions = new CookieOptions
        {
            HttpOnly = false, // Accessible to JS
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/user/auth/refresh", 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, options);
        Response.Cookies.Append("refreshCsrf", refreshCsrf, csrfOptions);
    }
}
