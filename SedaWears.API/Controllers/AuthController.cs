using MediatR;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Application.Features.Auth.Commands;
using SedaWears.Application.Features.Invitations.Commands;
using SedaWears.Application.Features.Invitations.Queries;
using SedaWears.Application.Common.Settings;
using SedaWears.Application.Common;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(ISender mediator, IOriginContext originContext) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (userId, role) = await mediator.Send(new LoginCommand(request.Email, request.Password, request.RememberMe));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role.ToString()),
        };

        var scheme = AuthConstants.GetSchemeForRole(role.ToString());
        var claimsIdentity = new ClaimsIdentity(claims, scheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = request.RememberMe,
            ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
        };

        await HttpContext.SignInAsync(
            scheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await mediator.Send(
            new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone)
        );
        return Ok(new { Message = "Registration successful. Please login to continue." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var scheme = AuthConstants.GetSchemeForRole(originContext.CurrentRole.ToString());
        await HttpContext.SignOutAsync(scheme);
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


}
