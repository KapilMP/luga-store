using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Features.Auth.Commands;
using SedaWears.Application.Features.Invitations.Commands;
using SedaWears.Application.Features.Invitations.Queries;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Controllers;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record AcceptInvitationRequest(int? ShopId, string Email, string Token, string FirstName, string LastName, string Password, UserRole Role);

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

    [HttpGet("dev/invite-token")]
    public async Task<IActionResult> DevGetInviteToken([FromQuery] string email, [FromQuery] string role, [FromServices] UserManager<SedaWears.Domain.Entities.User> userManager)
    {
        if (!environment.IsDevelopment()) return NotFound();
        if (!Enum.TryParse<SedaWears.Domain.Enums.UserRole>(role, true, out var roleEnum)) return BadRequest("Invalid role.");
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == roleEnum);
        if (user == null) return NotFound("User not found.");
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(new { email, role, token });
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
        return Ok(new { Message = "A reset password link has been sent to your email if it exists in our system." });
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
        await mediator.Send(new AcceptInvitationCommand(
            request.ShopId,
            request.Email,
            request.Token,
            request.FirstName,
            request.LastName,
            request.Password,
            request.Role));

        return Ok(new { Message = "Invitation accepted successfully. You can now login with your credentials." });
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
