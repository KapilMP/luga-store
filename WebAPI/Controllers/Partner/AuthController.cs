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
using UserEntity = LugaStore.Domain.Entities.User;
using LugaStore.Domain.Common;
using LugaStore.Domain.Enums;
using LugaStore.WebAPI.Controllers;

namespace LugaStore.WebAPI.Controllers.Partner;

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);

[ApiController]
[Route("partner/[controller]")] 
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

        // Role Validation: Partners, Admins, and Managers can login here
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null || 
            (!await userManager.IsInRoleAsync(user, Roles.Partner) && 
             !await userManager.IsInRoleAsync(user, Roles.Admin) && 
             !await userManager.IsInRoleAsync(user, Roles.Manager)))
        {
            return Forbid("Access denied: You are not authorized for the partner portal.");
        }

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
        {
            var refreshCsrf = Guid.NewGuid().ToString();
            SetAuthCookies(authResult.RefreshToken, refreshCsrf, "/partner/auth/refresh");
        }

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await authService.ForgotPasswordAsync(request.Email);
        return Ok("If the partner exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        if (!result) return BadRequest("Invalid Token or Partner");
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
        SetAuthCookies(result.Value.RefreshToken, newRefreshCsrf, "/partner/auth/refresh");

        return Ok(new { accessToken = result.Value.AccessToken });
    }

}
