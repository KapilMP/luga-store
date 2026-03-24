using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.WebAPI.Controllers.User;

public record NewsletterRequest(string Email);
public record UnsubscribeConfirmationRequest(string Token);

[ApiController]
[Route("user/[controller]")]
[AllowAnonymous]
public class NewsletterController(IApplicationDbContext context) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] NewsletterRequest request)
    {
        if (string.IsNullOrEmpty(request.Email)) return BadRequest("Email is required.");

        var existing = await context.Newsletters
            .FirstOrDefaultAsync(n => n.Email == request.Email);

        if (existing != null)
        {
            if (existing.IsSubscribed) return Ok("You are already subscribed!");
            
            existing.IsSubscribed = true;
            existing.UnsubscribeToken = Guid.NewGuid().ToString("N");
            await context.SaveChangesAsync(default);
            return Ok("Welcome back! Your subscription has been reactivated.");
        }

        var entry = new Newsletter
        {
            Email = request.Email,
            IsSubscribed = true,
            UnsubscribeToken = Guid.NewGuid().ToString("N")
        };

        context.Newsletters.Add(entry);
        await context.SaveChangesAsync(default);

        return Ok("Thank you for subscribing to Luga Store updates!");
    }

    [HttpGet("validate-unsubscribe/{token}")]
    public async Task<IActionResult> ValidateUnsubscribe(string token)
    {
        // Frontend calls this when the page loads to show "Confirm unsubscribe for {email}?"
        if (string.IsNullOrEmpty(token)) return BadRequest("Token is required.");

        var existing = await context.Newsletters
            .FirstOrDefaultAsync(n => n.UnsubscribeToken == token);

        if (existing == null) return NotFound("Invalid unsubscribe link.");

        return Ok(new { email = existing.Email });
    }

    [HttpPost("confirm-unsubscribe")]
    public async Task<IActionResult> ConfirmUnsubscribe([FromBody] UnsubscribeConfirmationRequest request)
    {
        // Final action when user clicks "Confirm" button on the frontend page
        if (string.IsNullOrEmpty(request.Token)) return BadRequest("Token is required.");

        var existing = await context.Newsletters
            .FirstOrDefaultAsync(n => n.UnsubscribeToken == request.Token);

        if (existing == null) return NotFound("Invalid or expired unsubscribe token.");

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(default);

        return Ok("You have been successfully unsubscribed.");
    }

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] NewsletterRequest request)
    {
        var existing = await context.Newsletters
            .FirstOrDefaultAsync(n => n.Email == request.Email);

        if (existing == null) return NotFound("Email not found.");

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(default);

        return Ok("You have been unsubscribed.");
    }
}
