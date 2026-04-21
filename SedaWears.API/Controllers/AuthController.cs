using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Application.Features.Auth.Commands;
using SedaWears.Application.Features.Auth.Queries;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOriginContext originContext,
    IOptions<JwtConfig> jwtOptions,
    IWebHostEnvironment environment,
    IAntiforgery antiforgery) : ControllerBase
{
    private string RefreshPath => originContext.RefreshPath;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new LoginCommand(request.Email, request.Password));
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (response, refreshToken) = await mediator.Send(
            new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone)
        );
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return Unauthorized("Invalid CSRF token");
        }

        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token");

        var (response, newRefreshToken) = await mediator.Send(new RefreshTokenCommand(refreshToken));
        SetAuthCookies(newRefreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookies(originContext.RefreshPath);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> Forgot(ForgotPasswordRequest req)
    {
        await mediator.Send(new ForgotPasswordCommand(req.Email));
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset(ResetPasswordRequest req)
    {
        await mediator.Send(new ResetPasswordCommand(req.Email, req.Token, req.NewPassword));
        return Ok();
    }

    [HttpPost("accept-invitation")]
    public async Task<IActionResult> AcceptInvitation(AcceptInvitationRequest request)
    {
        var (response, refreshToken) = await mediator.Send(new AcceptInvitationCommand(
            request.Email,
            request.Token,
            request.FirstName,
            request.LastName,
            request.Password,
            request.Role));
        
        SetAuthCookies(refreshToken, RefreshPath);
        return Ok(response);
    }

    [HttpGet("invitation-details")]
    public async Task<IActionResult> GetInvitationDetails([FromQuery] string email, [FromQuery] string token)
    {
        var response = await mediator.Send(new GetInvitationDetailsQuery(email, token));
        return Ok(response);
    }

    private void SetAuthCookies(string token, string path)
    {
        var secure = environment.IsProduction();
        var expiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays);

        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Strict,
            Path = path,
            Expires = expiry
        });

        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append("csrfRefreshToken", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            Secure = secure,
            SameSite = SameSiteMode.Strict,
            Path = path,
            Expires = expiry
        });
    }

    private void ClearAuthCookies(string path)
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = path });
        Response.Cookies.Delete("csrfRefreshToken", new CookieOptions { Path = path });
    }
}
