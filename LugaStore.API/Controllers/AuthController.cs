using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.Auth.Commands;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);

[ApiController]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(
    ISender mediator,
    IOptions<RefreshTokenPathsConfig> options,
    IOptions<JwtConfig> jwtOptions,
    ITokenService tokenService,
    IWebHostEnvironment environment) : ControllerBase
{
    private string CookiePath(string role) => role switch {
        Roles.Admin => options.Value.AdminRefreshPath,
        Roles.Partner => options.Value.PartnerRefreshPath,
        Roles.PartnerManager => options.Value.PartnerManagerRefreshPath,
        _ => options.Value.CustomerRefreshPath
    };

    [HttpPost("admin/auth/login")] public Task<IActionResult> AdminLogin(LoginRequest req) => Login(req, Roles.Admin);
    [HttpPost("customer/auth/login")] public Task<IActionResult> CustomerLogin(LoginRequest req) => Login(req, Roles.Customer);
    [HttpPost("partner/auth/login")] public Task<IActionResult> PartnerLogin(LoginRequest req) => Login(req, Roles.Partner);
    [HttpPost("manager/auth/login")] public Task<IActionResult> ManagerLogin(LoginRequest req) => Login(req, Roles.PartnerManager);

    private async Task<IActionResult> Login(LoginRequest request, string role)
    {
        var response = await mediator.Send(new LoginCommand(request.Email, request.Password, role));
        SetAuthCookies(response.RefreshToken, CookiePath(role));
        return Ok(response);
    }

    [HttpPost("customer/auth/register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await mediator.Send(new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName, request.Phone));
        SetAuthCookies(response.RefreshToken, CookiePath(Roles.Customer));
        return Ok(response);
    }

    [HttpPost("admin/auth/refresh")] public Task<IActionResult> AdminRefresh() => Refresh(Roles.Admin);
    [HttpPost("customer/auth/refresh")] public Task<IActionResult> CustomerRefresh() => Refresh(Roles.Customer);
    [HttpPost("partner/auth/refresh")] public Task<IActionResult> PartnerRefresh() => Refresh(Roles.Partner);
    [HttpPost("manager/auth/refresh")] public Task<IActionResult> ManagerRefresh() => Refresh(Roles.PartnerManager);

    private async Task<IActionResult> Refresh(string role)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("No refresh token");

        var response = await mediator.Send(new RefreshTokenCommand(refreshToken, role));
        SetAuthCookies(response.RefreshToken, CookiePath(role));
        return Ok(response);
    }

    [HttpPost("{role}/auth/logout")]
    public IActionResult Logout(string role) { ClearAuthCookies(CookiePath(role)); return NoContent(); }

    [HttpPost("auth/forgot-password")] 
    public async Task<IActionResult> Forgot(ForgotPasswordRequest req) 
    { 
        await mediator.Send(new ForgotPasswordCommand(req.Email)); 
        return Ok(); 
    }

    [HttpPost("auth/reset-password")] 
    public async Task<IActionResult> Reset(ResetPasswordRequest req) 
    { 
        await mediator.Send(new ResetPasswordCommand(req.Email, req.Token, req.NewPassword)); 
        return Ok(); 
    }

    private void SetAuthCookies(string token, string path)
    {
        var secure = environment.IsProduction();
        Response.Cookies.Append("refreshToken", token, new CookieOptions { HttpOnly = true, Secure = secure, SameSite = SameSiteMode.Strict, Path = path, Expires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays) });
    }

    private void ClearAuthCookies(string path)
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = path });
    }
}
