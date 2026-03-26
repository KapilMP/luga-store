using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;

namespace LugaStore.WebAPI.Controllers.Customer;

public record GoogleLoginRequest(string IdToken);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record GuestCheckoutRequest(string Email, string FirstName, string LastName, string Phone);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record ConfirmEmailRequest(string UserId, string Token);

[ApiController]
[Route("customer/[controller]")]
[EnableRateLimiting("auth")]
[Consumes("application/json")]
public class AuthController(ISender mediator) : BaseAuthController
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var authResult = await mediator.Send(command);
        if (authResult == null) return Unauthorized("Invalid credentials.");

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
            SetAuthCookies(authResult.RefreshToken, Guid.NewGuid().ToString(), "/customer/auth/refresh");

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await mediator.Send(new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName, request.Phone));
        if (!result) return Conflict("Email is already registered.");
        return Ok("Registration successful. Please verify your email.");
    }

    [HttpPost("guest-checkout")]
    public async Task<IActionResult> GuestCheckout(GuestCheckoutRequest request)
    {
        await mediator.Send(new GuestCheckoutCommand(request.Email, request.FirstName, request.LastName, request.Phone));
        return Ok();
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var authResult = await mediator.Send(new GoogleLoginCommand(request.IdToken));
        if (authResult == null) return Unauthorized("Invalid Google Token.");

        if (!string.IsNullOrEmpty(authResult.RefreshToken))
            SetAuthCookies(authResult.RefreshToken, Guid.NewGuid().ToString(), "/customer/auth/refresh");

        return Ok(new { accessToken = authResult.AccessToken });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email));
        return Ok("If the customer exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await mediator.Send(new ResetPasswordCommand(request.Email, request.Token, request.NewPassword));
        if (!result) return BadRequest("Invalid Token or Customer.");
        return Ok("Password has been reset.");
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(ConfirmEmailRequest request)
    {
        var result = await mediator.Send(new ConfirmEmailCommand(request.UserId, request.Token));
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

        SetAuthCookies(result.Value.RefreshToken, Guid.NewGuid().ToString(), "/customer/auth/refresh");
        return Ok(new { accessToken = result.Value.AccessToken });
    }
}
