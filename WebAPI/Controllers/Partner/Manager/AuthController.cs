using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.WebAPI.Dtos;

namespace LugaStore.WebAPI.Controllers.PartnerManager;

[ApiController]
[Route("partner/{partnerId:int}/manager/[controller]")]
[EnableRateLimiting("auth")]
[Consumes("application/json")]
public class AuthController(ISender mediator, ICookieSettings cookieSettings) : BaseAuthController(cookieSettings)
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await mediator.Send(new PartnerManagerLoginCommand(request.Email, request.Password));
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(CookieSettings.PartnerManagerRefreshPath);
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
        var csrfHeader = Request.Headers["X-XSRF-TOKEN"].ToString();

        if (string.IsNullOrEmpty(refreshToken) ||
            string.IsNullOrEmpty(refreshCsrfCookie) ||
            csrfHeader != refreshCsrfCookie)
            return Unauthorized("Invalid refresh request (CSRF failure).");

        var result = await mediator.Send(new RefreshTokenCommand(refreshToken));
        if (result == null) return Unauthorized("Refresh session expired.");

        SetAuthCookies(result.Value.RefreshToken, CookieSettings.PartnerManagerRefreshPath);
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
