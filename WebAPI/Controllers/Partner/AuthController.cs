using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.WebAPI.Dtos;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[EnableRateLimiting("auth")]
[Consumes("application/json")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(cookieSettings)
{
    [HttpPost("invite")]
    public async Task<IActionResult> InvitePartner(InvitePartnerCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Failed to invite partner.");
        return Ok("Invitation sent.");
    }

    [HttpPost("resend-invitation")]
    public async Task<IActionResult> ResendInvitation(ResendInvitationCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("User not found or already accepted invitation.");
        return Ok("Invitation resent.");
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await mediator.Send(new PartnerLoginCommand(request.Email, request.Password));
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(RefreshTokenPaths.PartnerRefreshPath);
        return NoContent();
    }

    [HttpPost("change-password")]
    [Authorize]
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
        return Ok("If the partner exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or Partner.");
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

        SetAuthCookies(result.Value.RefreshToken, RefreshTokenPaths.PartnerRefreshPath);
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
