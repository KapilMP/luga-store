using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.Auth.Commands;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers;

[ApiController]
[EnableRateLimiting(nameof(RateLimitingPolicies.Auth))]
public class AuthController(ISender mediator) : ControllerBase
{
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
}
