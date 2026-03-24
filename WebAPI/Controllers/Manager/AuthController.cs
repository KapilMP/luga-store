using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Identity.Commands;
using LugaStore.Application.Common.Interfaces;
using UserEntity = LugaStore.Domain.Entities.User;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;
using LugaStore.WebAPI.Controllers;

namespace LugaStore.WebAPI.Controllers.Manager;

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);

[ApiController]
[Route("manager/[controller]")]
public class AuthController(
    ISender mediator,
    IAuthService authService,
    UserManager<UserEntity> userManager) : BaseAuthController
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var authResult = await mediator.Send(command);
        if (authResult == null) return Unauthorized("Invalid credentials.");

        // Role Validation: Ensure the user trying to log in via Manager path HAS the Manager or Admin role
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null || (!await userManager.IsInRoleAsync(user, Roles.Manager) && !await userManager.IsInRoleAsync(user, Roles.Admin)))
        {
            return Forbid("Access denied: You do not have management privileges.");
        }

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
        {
            var refreshCsrf = Guid.NewGuid().ToString();
            SetAuthCookies(authResult.RefreshToken, refreshCsrf, "/manager/auth/refresh");
        }

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await authService.ForgotPasswordAsync(request.Email);
        return Ok("If the manager exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        if (!result) return BadRequest("Invalid Token or Manager");
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
        {
            return Unauthorized("Invalid refresh request (CSRF failure)");
        }

        var result = await authService.RefreshTokenAsync(refreshToken);
        if (result == null)
            return Unauthorized("Refresh session expired");

        var newRefreshCsrf = Guid.NewGuid().ToString();
        SetAuthCookies(result.Value.RefreshToken, newRefreshCsrf, "/manager/auth/refresh");

        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
