using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.WebAPI.Dtos;

namespace LugaStore.WebAPI.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[EnableRateLimiting("auth")]
[Consumes("application/json")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(cookieSettings)
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await mediator.Send(new CustomerLoginCommand(request.Email, request.Password));
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginCommand command)
    {
        var result = await mediator.Send(command);
        if (result == null) return Unauthorized("Invalid Google Token.");
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(RefreshTokenPaths.CustomerRefreshPath);
        return NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return Conflict("Email is already registered.");
        return Ok("Registration successful. Please verify your email.");
    }

    [HttpPost("guest-checkout")]
    public async Task<IActionResult> GuestCheckout(GuestCheckoutCommand command)
    {
        await mediator.Send(command);
        return Ok();
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
        return Ok("If the customer exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or Customer.");
        return Ok("Password has been reset.");
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(ConfirmEmailCommand command)
    {
        var result = await mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or Customer.");
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
            return Unauthorized("Invalid refresh request (CSRF failure).");

        var result = await mediator.Send(new RefreshTokenCommand(refreshToken));
        if (result == null) return Unauthorized("Refresh session expired.");

        SetAuthCookies(result.Value.RefreshToken, RefreshTokenPaths.CustomerRefreshPath);
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
