using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.WebAPI.Dtos;

namespace LugaStore.WebAPI.Controllers.Manager;

[ApiController]
[Route("manager/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(cookieSettings)
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await mediator.Send(new PartnerManagerLoginCommand(request.Email, request.Password));
        SetAuthCookies(result.RefreshToken, RefreshTokenPaths.PartnerManagerRefreshPath);
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(RefreshTokenPaths.PartnerManagerRefreshPath);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("If the manager exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or PartnerManager.");
        return Ok("Password has been reset.");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh()
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

        SetAuthCookies(result.Value.RefreshToken, RefreshTokenPaths.PartnerManagerRefreshPath);
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
