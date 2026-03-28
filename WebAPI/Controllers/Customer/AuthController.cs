using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Identity.Commands;
using LugaStore.Infrastructure.Settings;
using LugaStore.Application.Identity;

namespace LugaStore.WebAPI.Controllers.Customer;

[Route("customer/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(ISender mediator, IRefreshTokenPaths cookieSettings) : BaseAuthController(mediator, cookieSettings)
{
    protected override string AuthRefreshPath => RefreshTokenPaths.CustomerRefreshPath;

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await Mediator.Send(new CustomerLoginCommand(request.Email, request.Password));
        SetAuthCookies(result.RefreshToken, AuthRefreshPath);
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginCommand command)
    {
        var result = await Mediator.Send(command);
        if (result == null) return Unauthorized("Invalid Google Token.");
        SetAuthCookies(result.RefreshToken, AuthRefreshPath);
        return Ok(new { accessToken = result.AccessToken, user = result.User });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result) return Conflict("Email is already registered.");
        return Ok("Registration successful. Please verify your email.");
    }

    [HttpPost("guest-checkout")]
    public async Task<IActionResult> GuestCheckout(GuestCheckoutCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(ConfirmEmailCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result) return BadRequest("Invalid Token or Customer.");
        return Ok("Email has been verified.");
    }
}
