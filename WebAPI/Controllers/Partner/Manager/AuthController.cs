using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Domain.Common;
using UserEntity = LugaStore.Domain.Entities.User;

namespace LugaStore.WebAPI.Controllers.PartnerManager;

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);

[ApiController]
[Route("partner/{partnerId:int}/manager/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(
    ISender mediator,
    UserManager<UserEntity> userManager) : BaseAuthController
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var authResult = await mediator.Send(command);
        if (authResult == null) return Unauthorized("Invalid credentials.");

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null ||
            (!await userManager.IsInRoleAsync(user, Roles.PartnerManager) &&
             !await userManager.IsInRoleAsync(user, Roles.Admin)))
            return Forbid("Access denied: You do not have management privileges.");

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
            SetAuthCookies(authResult.RefreshToken, Guid.NewGuid().ToString(), "/partner/{partnerId}/manager/auth/refresh");

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email));
        return Ok("If the manager exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await mediator.Send(new ResetPasswordCommand(request.Email, request.Token, request.NewPassword));
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

        SetAuthCookies(result.Value.RefreshToken, Guid.NewGuid().ToString(), "/partner/{partnerId}/manager/auth/refresh");
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
