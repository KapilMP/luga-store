using MediatR;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.NewsletterFeature.Commands;
using LugaStore.Application.NewsletterFeature.Queries;

namespace LugaStore.WebAPI.Controllers.Customer;

public record NewsletterRequest(string Email);
public record UnsubscribeConfirmationRequest(string Token);

[ApiController]
[Route("customer/[controller]")]
public class NewsletterController(ISender mediator) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] NewsletterRequest request)
    {
        var result = await mediator.Send(new SubscribeCommand(request.Email));
        return result switch
        {
            "already_subscribed" => Ok("You are already subscribed!"),
            "reactivated" => Ok("Welcome back! Your subscription has been reactivated."),
            _ => Ok("Thank you for subscribing to Luga Store updates!")
        };
    }

    [HttpGet("validate-unsubscribe/{token}")]
    public async Task<IActionResult> ValidateUnsubscribe(string token)
    {
        var email = await mediator.Send(new ValidateUnsubscribeQuery(token));
        if (email == null) return NotFound("Invalid unsubscribe link.");
        return Ok(new { email });
    }

    [HttpPost("confirm-unsubscribe")]
    public async Task<IActionResult> ConfirmUnsubscribe([FromBody] UnsubscribeConfirmationRequest request)
    {
        var result = await mediator.Send(new ConfirmUnsubscribeCommand(request.Token));
        if (!result) return NotFound("Invalid or expired unsubscribe token.");
        return Ok("You have been successfully unsubscribed.");
    }

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] NewsletterRequest request)
    {
        var result = await mediator.Send(new UnsubscribeCommand(request.Email));
        if (!result) return NotFound("Email not found.");
        return Ok("You have been unsubscribed.");
    }
}
